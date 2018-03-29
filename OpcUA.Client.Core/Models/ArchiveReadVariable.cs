using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class ArchiveReadVariable
    {
        public int VariableId { get; set; } 
        public object Value { get; set; }
        public NodeId RegisteredNodeId { get; set; }
        public ServiceResult Result { get; set; }
        public BuiltInType Type { get; set; }
        public ArchiveInterval Interval { get; set; }
    }
}
