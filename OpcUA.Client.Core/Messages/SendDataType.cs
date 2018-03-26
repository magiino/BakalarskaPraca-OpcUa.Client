using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class SendDataType
    {
        public BuiltInType Type { get; set; }

        public SendDataType(BuiltInType type)
        {
            Type = type;
        }
    }
}
