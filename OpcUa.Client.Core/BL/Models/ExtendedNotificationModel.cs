using System;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class ExtendedNotificationModel : BaseViewModel
    {
        public string Name { get; set; }
        public string NodeId { get; set; }

        public object Value { get; set; }
        public BuiltInType DataType { get; set; }
        public StatusCode StatusCode { get; set; }
        public DateTime SourceDateTime { get; set; }
    }
}
