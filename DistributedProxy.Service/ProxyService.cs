using System.ServiceProcess;
using DistributedProxy.Application;

namespace DistributedProxy.Service
{
    public partial class ProxyService : ServiceBase
    {
        public ProxyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var proxy = new ProxyHandler();
            var threadHandler = new ThreadHandler();
            var installHandler = new InstallHandler();
            proxy.Start();
            threadHandler.StartRequestThread();
            installHandler.Install();
        }

        protected override void OnStop()
        {
            var proxy = new ProxyHandler();
            var threadHandler = new ThreadHandler();
            var installHandler = new InstallHandler();
            threadHandler.StopRequestThread();
            proxy.Stop();
            installHandler.Uninstall(true);
        }
    }
}
