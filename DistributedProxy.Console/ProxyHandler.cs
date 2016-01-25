using System;
using System.Net;
using System.Threading;
using DistributedProxy.Console.Model;

namespace DistributedProxy.Console
{
    class ProxyHandler
    {
        private HttpListenerContext Context { get; }

        public ProxyHandler(HttpListenerContext inputContext)
        {
            Context = inputContext;
        }

        public void HandleRequest()
        {
            
            //Check caching local
            //Check caching list
            //Submit request
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(Context.Request.RawUrl);
            myHttpWebRequest.KeepAlive = false;
            myHttpWebRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            myHttpWebRequest.UserAgent = Context.Request.UserAgent;
            var myRequestState = new RequestState(myHttpWebRequest, Context);
            myHttpWebRequest.BeginGetResponse(new AsyncCallback(Response), myRequestState);
            Thread.Sleep(10000);
        }

        private static void Response(IAsyncResult asynchronousResult)
        {
            System.Console.WriteLine("a");
            var requestData = (RequestState)asynchronousResult.AsyncState;
            using (var responseFromWebSiteBeingRelayed = (HttpWebResponse)requestData.Request.EndGetResponse(asynchronousResult))
            {
                using (var stream = responseFromWebSiteBeingRelayed.GetResponseStream())
                {        
                    stream.Close();
                }
            }
        }
    }
}