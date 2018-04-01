namespace OpcUa.Client.Core
{
    public class ProjectRepository : BaseRepository<ProjectEntity>, IProjectRepository
    {
        public ProjectRepository(DataContext dataDontext): base(dataDontext) {}
    }
}