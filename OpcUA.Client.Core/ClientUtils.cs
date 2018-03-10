using System.Reflection;
using System.ServiceModel.Configuration;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUA.Client.Core
{
    static class ClientUtils
    {
        public static NodeId GetDataType(Session session,NodeId nodeId)
        {
            Node node = session.ReadNode(nodeId);
            var dataTypeNode = (node.DataLock as VariableNode)?.DataType;
            session.ReadNode(dataTypeNode);
            return dataTypeNode;
        }

    }
}
