using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class NotificationEntity : BaseEntity
    {
        public string Name { get; set; }
        public string NodeId { get; set; }
        public double FilterValue { get; set; }
        public DeadbandType DeadbandType { get; set; }
        public bool IsDigital { get; set; }
        public string IsZeroDescription { get; set; }
        public string IsOneDescription { get; set; }
    }
}