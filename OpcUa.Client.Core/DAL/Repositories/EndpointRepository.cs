namespace OpcUa.Client.Core
{
    public class EndpointRepository : BaseRepository<EndpointEntity>, IEndpointRepository
    {
        public EndpointRepository(DataContext dataDontext): base(dataDontext) {}
    }
}