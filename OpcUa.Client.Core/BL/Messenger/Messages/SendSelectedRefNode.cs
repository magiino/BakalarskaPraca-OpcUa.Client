using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class SendSelectedRefNode
    {
        public ReferenceDescription ReferenceNode { get; set; }

        public SendSelectedRefNode(ReferenceDescription referenceNode)
        {
            ReferenceNode = referenceNode;
        }
    }
}
