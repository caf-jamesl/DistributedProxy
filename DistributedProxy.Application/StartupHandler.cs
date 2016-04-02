
namespace DistributedProxy.Application
{
    public class StartupHandler
    {
        public StartupHandler()
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
