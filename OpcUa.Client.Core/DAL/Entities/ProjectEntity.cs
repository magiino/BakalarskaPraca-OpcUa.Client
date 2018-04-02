namespace OpcUa.Client.Core
{
    public class ProjectEntity : BaseEntity
    {
        public string Name { get; set; }
        public int EndpointId { get; set; }
        public EndpointEntity Endpoint { get; set; }
        public string SessionName { get; set; }
        public int? UserId { get; set; }
        public UserEntity User { get; set; }
    }
}