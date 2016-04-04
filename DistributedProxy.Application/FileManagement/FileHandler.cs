using System;
using System.IO;
using System.Net;

namespace DistributedProxy.Application.FileManagement
{
    public class FileHandler
    {
        internal void SaveCachedFile(Stream stream, WebRequest webRequest)
        {
            stream.Position = 0;
            var filename = Guid.NewGuid();
            var fileLocation = $@"C:\proxy\cache\{filename}";
            using (var fileStream = new FileStream(fileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.CopyTo(fileStream);
            }
            stream.Close();
            XmlRecordFile.Instance.SaveCachedFileRecord(webRequest.RequestUri.AbsoluteUri, fileLocation, ConnectionHandler.IpAddress);
        }

        public static void ClearFiles()
        {
            var di = new DirectoryInfo(@"C:\proxy\cache\");
            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}