using System.Net;
using System.Threading;

namespace DistributedProxy.Console
{
    public class Program
    {
        private static void Main()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://+:7777/");
            listener.Start();
            System.Console.WriteLine("Listening...");
            while (listener.IsListening)
            {
                var context = listener.GetContext();
                new Thread(new ProxyHandler(context).HandleRequest).Start();
            }
        }
    }
}