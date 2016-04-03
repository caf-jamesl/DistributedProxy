using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;
using System.Configuration;
using System.Xml.Linq;


namespace DistributedProxy.Application
{
    public class InstallHandler
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        public void Install()
        {
            if(!File.Exists(ConfigurationManager.AppSettings["xmlDocumentLocation"]))
            {
                Directory.CreateDirectory(@"C:\proxy\cache");
                new XDocument(new XElement("Resources")).Save(ConfigurationManager.AppSettings["xmlDocumentLocation"]);
            }
            var registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            if (registry == null) return;
            registry.SetValue("ProxyEnable", 1);
            registry.SetValue("ProxyServer", "127.0.0.1:7777");
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            registry.Close();
        }

        public void Uninstall()
        {
            var registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            if (registry == null) return;
            registry.SetValue("ProxyEnable", 0);
            registry.SetValue("ProxyServer", "127.0.0.1:7777");
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
            registry.Close();
            ConnectionHandler.SendHostLeave();
        }
    }
}