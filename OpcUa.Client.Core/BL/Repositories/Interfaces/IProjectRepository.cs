using System.Collections.Generic;

namespace OpcUa.Client.Core
{
    public interface IProjectRepository : IBaseRepository<ProjectEntity>
    {
        new IEnumerable<ProjectEntity> GetAllWithEndpoints();
    }
}