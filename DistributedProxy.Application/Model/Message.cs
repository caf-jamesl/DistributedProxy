using System;
using DistributedProxy.Application.Model.Enum;

namespace DistributedProxy.Application.Model
{
    [Serializable]
    internal class Message
    {
        internal MessageType Type { get; set; }
        internal string Content { get; set; }
    }
}
