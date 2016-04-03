using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace DistributedProxy.Application.FileManagement
{
    public sealed class XmlRecordFile
    {
        private static readonly object Padlock = new object();
        private static XmlRecordFile _instance;

        public static XmlRecordFile Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new XmlRecordFile();
                    }
                }
                return _instance;
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
                               where (string)resource.Element("Address") == address
                               select resource.Element("Location")?.Value;
                var enumerable = location as string[] ?? location.ToArray();
                return enumerable.Any() ? enumerable.First() : null;
            }
        }

        public List<string> GetLocalCachedItems()
        {
            lock (Padlock)
            {
                var xmlDocumentLocation = ConfigurationManager.AppSettings["xmlDocumentLocation"];
                var document = XDocument.Load(xmlDocumentLocation);
                var elements = from resource in document.Descendants("Resource")
                               where (string)resource.Element("Machine") == ConnectionHandler.IpAddress
                               select resource;
                var xElements = elements as IList<XElement> ?? elements.ToList();
                foreach (var element in xElements)
                {
                    var location = element.Element("Location")?.Value;
                    var networkedLocation = location?.Replace(@"C:", @"\\" + ConnectionHandler.IpAddress + @"\c$");
                    var xElement = element.Element("Location");
                    if (xElement == null) continue;
                    if (networkedLocation != null) xElement.Value = networkedLocation;
                }
                return xElements.Select(element => element.ToString()).ToList();
            }
        }

        public void AddToCacheListFromRemote(List<string> elements)
        { 
            var xElements = elements.Select(XElement.Parse).ToList();
            lock (Padlock)
            {
                var xmlDocumentLocation = ConfigurationManager.AppSettings["xmlDocumentLocation"];
                var document = XDocument.Load(xmlDocumentLocation);
                foreach (var xElement in xElements)
                {
                    document.Root?.Add(xElement);
                }
                document.Save(xmlDocumentLocation);
            }
        }

        public void RemoveCacheListFromRemote(string ip)
        {
            lock (Padlock)
            {
                var xmlDocumentLocation = ConfigurationManager.AppSettings["xmlDocumentLocation"];
                var document = XDocument.Load(xmlDocumentLocation);
                foreach (var element in document.Descendants("Resource").Where(element => element.Element("Machine")?.Value == ip))
                {
                    element.Remove();
                }
                document.RemoveNodes();
                document.Save(xmlDocumentLocation);
            }
        }
    }
}