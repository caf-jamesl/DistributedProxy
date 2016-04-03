using System;
using System.Threading.Tasks;
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
            new Task(StartupHandler.Start).Start();
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Main());
        }
    }
}