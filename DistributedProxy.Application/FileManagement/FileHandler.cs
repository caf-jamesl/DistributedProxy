using System;
using System.IO;
using System.Net;

namespace DistributedProxy.Application.FileManagement
{
    internal class FileHandler
    {
        internal void SaveCachedFile(Stream stream, WebRequest webRequest)
        {
            stream.Position = 0;
            var filename = Guid.NewGuid();
            var fileLocation = $@"C:\cache\{filename}";
            using (var fileStream = new FileStream(fileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                stream.CopyTo(fileStream);
            }
            stream.Close();
            XmlRecordFile.Instance.SaveCachedFileRecord(webRequest.RequestUri.AbsoluteUri, fileLocation);
        }
    }
}