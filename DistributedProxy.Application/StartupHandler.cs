using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace DistributedProxy.Application
{
    public static class StartupHandler
    {
        public static void Start()
        {
            if (!File.Exists(ConfigurationManager.AppSettings["xmlDocumentLocation"]))
            {
                Directory.CreateDirectory(@"C:\proxy\cache");
                new XDocument(new XElement("Resources")).Save(ConfigurationManager.AppSettings["xmlDocumentLocation"]);
            }
            var connectionHandler = new ConnectionHandler();
            connectionHandler.SetupTcp();
            connectionHandler.SetupUdp();
            connectionHandler.SendNewHostSignal();
            var threadHandler = new ThreadHandler();
            threadHandler.StartNewHostThread();
        }
    }
}