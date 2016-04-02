using System.Threading;

namespace DistributedProxy.Application
{
    public class ThreadHandler
    {
        internal static bool RequestThreadIsRunning { get; set; }
        internal static bool NewHostThreadIsRunning { get; set; }
        private static Thread RequestThread  { get; set; }
        private static Thread NewHostThread { get; set; }

        public ThreadHandler()
        {
            if (RequestThread == null || !RequestThread.IsAlive)
            {
                RequestThread = new Thread(new ProxyHandler().CheckRequests);
            }

            if (NewHostThread == null)
            {
                NewHostThread = new Thread(new ConnectionHandler().CheckForNewHosts);
            }
        }
        public void StartRequestThread()
        {
            RequestThread.Start();
            RequestThreadIsRunning = true;
            ProxyHandler.IsChecking = true;
        }

        public void StopRequestThread()
        {
            RequestThreadIsRunning = false;
            ProxyHandler.IsChecking = false;
            RequestThread.Interrupt();
        }

        public void StartNewHostThread()
        {
            NewHostThread.Start();
            NewHostThreadIsRunning = true;
            ConnectionHandler.IsCheckingForNewHosts = true;
        }

        public void StopNewHostThread()
        {
            NewHostThreadIsRunning = false;
            ConnectionHandler.IsCheckingForNewHosts = false;
            NewHostThread.Interrupt();
        }
    }
}
