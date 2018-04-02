using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class EndpointEntity : BaseEntity
    {
        public string Url { get; set; }
        public string SecurityPolicyUri { get; set; }
        public MessageSecurityMode MessageSecurityMode { get; set; }
        public string TransportProfileUri { get; set; }
    }
}