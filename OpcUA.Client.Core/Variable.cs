using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class Variable : BaseViewModel
    {
        public NodeId NodeId { get; set; }

        public string Name => NodeId.ToString();

        public object Value { get; set; }

        public string DataType { get; set; }

        public Variable(NodeId nodeId)
        {
            NodeId = nodeId;
            DataType = IoC.Get<UaClientApi>().GetDataType(NodeId).ToString();
        }
    }
}
