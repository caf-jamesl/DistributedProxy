using System.ServiceProcess;
using System.Threading.Tasks;
using DistributedProxy.Application;

namespace DistributedProxy.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            new Task(StartupHandler.Start).Start();
            var servicesToRun = new ServiceBase[]
            {
                new ProxyService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
