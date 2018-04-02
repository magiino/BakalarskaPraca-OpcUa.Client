namespace OpcUa.Client.Core
{
    public class ProjectModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EndpointId { get; set; }
        public string EndpointUrl { get; set; }
        public string SessionName { get; set; }
        public int? UserId { get; set; }
    }
}
