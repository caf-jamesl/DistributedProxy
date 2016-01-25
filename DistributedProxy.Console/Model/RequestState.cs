using System.IO;
using System.Net;
using System.Text;

namespace DistributedProxy.Console.Model
{
    public class RequestState
    {
        // This class stores the State of the request.
        const int BufferSize = 1024;
        public StringBuilder RequestData { get; }
        public byte[] BufferRead { get; }
        public HttpWebRequest Request { get; }
        public HttpListenerContext Context { get; }
        public Stream StreamResponse { get; }
        public RequestState(HttpWebRequest request, HttpListenerContext context)
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder("");
            Request = null;
            StreamResponse = null;
            Request = request;
            Context = context;
        }
    }
}
