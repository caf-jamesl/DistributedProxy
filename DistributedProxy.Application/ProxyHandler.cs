using System;
using System.Net;
using System.Threading.Tasks;

namespace DistributedProxy.Application
{
    public class ProxyHandler
    {
        public static bool IsRunning { get; set; }
        public static bool IsChecking { get; set; }

        private static HttpListener Listener { get; set; }


        public ProxyHandler()
        {
            if (Listener != null) return;
            Listener = new HttpListener();
            Listener.Prefixes.Add("http://+:7777/");
        }
        public void Stop()
        {
            Listener.Stop();
            IsRunning = false;
        }

        public void Start()
        {
            Listener.Start();
            IsRunning = true;
        }

        public void CheckRequests()
        {
            while (IsChecking)
            {
                try
                {
                    var context = Listener.GetContext();
                    new Task(() => new RequestHandler(context).HandleRequest()).Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
