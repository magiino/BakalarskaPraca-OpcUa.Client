using System.Collections.Generic;

namespace OpcUa.Client.Core
{
    public interface IRecordRepository : IBaseRepository<RecordEntity>
    {
        IEnumerable<RecordEntity> Local();
    }
}