namespace OpcUa.Client.Core
{
    public class ProjectEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public double SessionName { get; set; }
        public int? UserEntityId { get; set; }
        public UserEntity User { get; set; }
    }
}