using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class SendSelectedRefNode
    {
        public ReferenceDescription RefNode { get; set; }

        public SendSelectedRefNode(ReferenceDescription refNode)
        {
            RefNode = refNode;
        }
    }
}
