using System.Collections.Generic;
using System.Data.Entity;

namespace OpcUa.Client.Core
{
    public class ProjectRepository : BaseRepository<ProjectEntity>, IProjectRepository
    {
        private DataContext DataContect => Context as DataContext;
        public ProjectRepository(DataContext dataDontext): base(dataDontext) {}

        public IEnumerable<ProjectEntity> GetAllWithEndpoints()
        {
            return DataContect.Projects.Include(x => x.Endpoint);
        }
    }
}