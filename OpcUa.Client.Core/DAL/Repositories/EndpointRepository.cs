namespace OpcUa.Client.Core
{
    public class EndpointRepository : BaseRepository<EndpointEntity>, IEndpointRepository
    {
        private DataContext DataContect => Context as DataContext;
        public EndpointRepository(DataContext dataDontext): base(dataDontext) {}
    }
}