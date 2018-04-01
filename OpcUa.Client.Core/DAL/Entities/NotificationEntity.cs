namespace OpcUa.Client.Core
{
    public class NotificationEntity : BaseEntity
    {
        public string Name { get; set; }
        public string NodeId { get; set; }
        public double FilterValue { get; set; }
        public string Description { get; set; }

    }
}