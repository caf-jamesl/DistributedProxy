using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using DistributedProxy.Application.Model.Enum;

namespace DistributedProxy.Application.Model
{
    [Serializable]
    internal class XmlMessage : Message
    {
        internal new MessageType Type { get; set; }
        internal new List<string> Content { get; set; }
    }
}
