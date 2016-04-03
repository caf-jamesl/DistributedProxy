
namespace DistributedProxy.Application
{
    public static class StartupHandler
    {
        public static void Start()
        {
            var connectionHandler = new ConnectionHandler();
            connectionHandler.SetupTcp();
            connectionHandler.SetupUdp();
            connectionHandler.SendNewHostSignal();
            var threadHandler = new ThreadHandler();
            threadHandler.StartNewHostThread();
        }
    }
}