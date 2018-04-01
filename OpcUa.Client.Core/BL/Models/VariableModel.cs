using System;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class VariableModel : BaseViewModel
    {
        public string NodeId { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        public BuiltInType DataType { get; set; }

        public StatusCode StatusCode { get; set; }

        public DateTime ServerDateTime { get; set; }
        public DateTime SourceDateTime { get; set; }
    }
}
