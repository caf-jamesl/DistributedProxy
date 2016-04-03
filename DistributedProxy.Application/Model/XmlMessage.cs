using System;
using System.Collections.Generic;

namespace DistributedProxy.Application.Model
{
    [Serializable]
    internal class XmlMessage : Message
    {
        internal new List<string> Content { get; set; }
    }
}
