using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class EndpointEntity : BaseEntity
    {
        [Required]
        public string Url { get; set; }
        [Required]
        public string SecurityPolicyUri { get; set; }
        public MessageSecurityMode MessageSecurityMode { get; set; }
        [Required]
        public string TransportProfileUri { get; set; }
    }
}