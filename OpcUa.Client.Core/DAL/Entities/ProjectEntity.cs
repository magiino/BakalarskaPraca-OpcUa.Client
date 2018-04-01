namespace OpcUa.Client.Core
{
    public class ProjectEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string SessionName { get; set; }
        public int? UserId { get; set; }
        public UserEntity User { get; set; }
    }
}