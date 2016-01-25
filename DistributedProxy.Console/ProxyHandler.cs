using System;
using System.IO;
using System.Net;
using System.Text;
using DistributedProxy.Console.Model;

namespace DistributedProxy.Console
{
    internal class ProxyHandler
    {
        private HttpListenerContext Context { get; }

        public ProxyHandler(HttpListenerContext inputContext)
        {
            Context = inputContext;
        }

        public void HandleRequest()
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(Context.Request.RawUrl);
            System.Console.WriteLine(Context.Request.RawUrl);
            webRequest.KeepAlive = false;
            webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webRequest.UserAgent = Context.Request.UserAgent;
            webRequest.Proxy = new WebProxy();
            var myRequestState = new RequestState(webRequest, Context);
            webRequest.BeginGetResponse(Response, myRequestState);
        }

        private static void Response(IAsyncResult asynchronousResult)
        {
            try
            {
                var requestData = (RequestState)asynchronousResult.AsyncState;
                using (var httpWebResponse = (HttpWebResponse)requestData.Request.EndGetResponse(asynchronousResult))
                {
                    using (var responseStream = httpWebResponse.GetResponseStream())
                    {
                        var originalResponse = requestData.Context.Response;

                        if (httpWebResponse.ContentType.Contains("text/html"))
                        {
                            var reader = new StreamReader(responseStream);
                            var html = reader.ReadToEnd();
                            var byteArray = Encoding.Default.GetBytes(html);
                            var stream = new MemoryStream(byteArray);
                            stream.CopyTo(originalResponse.OutputStream);
                        }
                        else
                        {
                            responseStream.CopyTo(originalResponse.OutputStream);
                        }
                        originalResponse.OutputStream.Close();
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}