using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Caching;
using DistributedProxy.Application.FileManagement;

namespace DistributedProxy.Application
{
    internal class RequestHandler
    {
        private HttpListenerContext Context { get; }

        internal RequestHandler(HttpListenerContext inputContext)
        {
            Context = inputContext;
            ServicePointManager.Expect100Continue = false;
        }

        internal void HandleRequest()
        {
            if (Context.Request.IsSecureConnection)
            {
            return;    
            }
            var location = XmlRecordFile.Instance.CheckForCachedFile(Context.Request.RawUrl);
            if (location != null)
            {
                using (var outputStream = Context.Response.OutputStream)
                {
                    using (var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read))
                    {
                        Context.Response.KeepAlive = Context.Request.KeepAlive;
                        Context.Response.ContentType = Context.Request.ContentType;
                        Context.Response.ContentEncoding = Context.Request.ContentEncoding;
                        Context.Response.StatusCode = 200;
                        Context.Response.ContentLength64 = fileStream.Length;
                        Context.Response.ProtocolVersion = Context.Request.ProtocolVersion;
                        fileStream.CopyTo(outputStream);
                    }
                }
                return;
            }
            var webRequest = (HttpWebRequest)WebRequest.Create(Context.Request.RawUrl);
            webRequest.KeepAlive = Context.Request.KeepAlive;
            webRequest.UserAgent = Context.Request.UserAgent;
            webRequest.ContentType = Context.Request.ContentType;
            webRequest.ContentLength = Context.Request.ContentLength64;
            webRequest.Method = Context.Request.HttpMethod;
            webRequest.Proxy = null;
            if (webRequest.Method == "POST")
            {
                HandlePost(ref webRequest);
            }
            var newStream = new MemoryStream();
            using (var responce = webRequest.GetResponse())
            {
                using (var responseStream = responce.GetResponseStream())
                {
                    responseStream?.CopyTo(newStream);
                    newStream.Position = 0;
                    using (var outputStream = Context.Response.OutputStream)
                    {
                        newStream.CopyTo(outputStream);
                    }
                }
            }
            new Task(() => new FileHandler().SaveCachedFile(newStream, webRequest)).Start();

        }

        private void HandlePost(ref HttpWebRequest webRequest)
        {
            using (var dataStream = webRequest.GetRequestStream())
            {
                var ms = new MemoryStream();
                Context.Request.InputStream.CopyTo(ms);
                var array = ms.ToArray();
                dataStream.Write(array, 0, array.Length);
            }
        }
    }
}