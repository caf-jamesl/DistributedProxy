using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace DistributedProxy.Application.FileManagement
{
    public sealed class XmlRecordFile
    {
        private static readonly object Padlock = new object();
        private static XmlRecordFile instance;

        public static XmlRecordFile Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (instance == null)
                    {
                        instance = new XmlRecordFile();
                    }
                }
                return instance;
            }
        }

        internal void SaveCachedFileRecord(string address, string filename, string machine)
        {
            lock (Padlock)
            {
                var xmlDocumentLocation = ConfigurationManager.AppSettings["xmlDocumentLocation"];
                var document = XDocument.Load(xmlDocumentLocation);
                var resource =
                    new XElement("Resource",
                        new XElement("Address", address),
                        new XElement("Location", filename),
                        new XElement("Machine", machine)
                        );
                document.Root?.Add(resource);
                document.Save(xmlDocumentLocation);
            }
        }

        internal string CheckForCachedFile(string address)
        {
            lock (Padlock)
            {
                var xmlDocumentLocation = ConfigurationManager.AppSettings["xmlDocumentLocation"];
                var document = XDocument.Load(xmlDocumentLocation);
                var location = from resource in document.Descendants("Resource")
                    where (string) resource.Element("Address") == address
                    select resource.Element("Location")?.Value;
                var enumerable = location as string[] ?? location.ToArray();
                return enumerable.Any() ? enumerable.First() : null;
            }
        }
    }
}