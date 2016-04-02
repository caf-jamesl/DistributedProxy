using System;
using DistributedProxy.Application;

namespace DistributedProxy.Form
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            new StartupHandler();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Main());
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            var installHandler = new InstallHandler();
            installHandler.Uninstall(); 
        }
    }
}