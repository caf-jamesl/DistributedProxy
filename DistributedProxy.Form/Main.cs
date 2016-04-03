using System;
using DistributedProxy.Application;

namespace DistributedProxy.Form
{
    public partial class Main : System.Windows.Forms.Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void proxyToggleBtn_Click(object sender, EventArgs e)
        {
            var proxy = new ProxyHandler();
            var threadHandler = new ThreadHandler();
            var installHandler = new InstallHandler();
            if (ProxyHandler.IsRunning)
            {
                threadHandler.StopRequestThread();
                proxy.Stop();
                installHandler.Uninstall();
                runProxyLbl.Text = @"Proxy not running";
            }
            else
            {
                proxy.Start();
                threadHandler.StartRequestThread();
                installHandler.Install();
                runProxyLbl.Text = @"Proxy running";
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            var installHandler = new InstallHandler();
            installHandler.Uninstall();
        }
    }
}
