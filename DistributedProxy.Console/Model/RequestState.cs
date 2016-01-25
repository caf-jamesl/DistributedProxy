using System.Net;

namespace DistributedProxy.Console.Model
{
    public class RequestState
    {
        public HttpWebRequest Request { get; }
        public HttpListenerContext Context { get; }

        public RequestState(HttpWebRequest request, HttpListenerContext context)
        {
            Request = request;
            Context = context;
        }
    }
}
