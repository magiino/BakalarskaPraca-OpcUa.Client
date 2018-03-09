using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class Variable
    {
        public NodeId NodeId { get; set; }

        public string Name => NodeId.ToString();

        public object Value { get; set; }

        //public string DataType => ClientUtils.GetDataType(NodeId).ToString();
    }
}
