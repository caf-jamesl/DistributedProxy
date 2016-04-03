using System;
using System.Collections.Generic;
using DistributedProxy.Application.Model.Enum;

namespace DistributedProxy.Application.Model
{
    [Serializable]
    internal class Message
    {
        internal MessageType Type { get; set; }
        internal string StringContent { get; set; }
        internal List<string> ListStringContent { get; set; }
    }
}
