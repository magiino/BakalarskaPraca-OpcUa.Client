using System.Collections.Generic;
using System.Linq;

namespace OpcUa.Client.Core
{
    public class RecordRepository : BaseRepository<RecordEntity>, IRecordRepository
    {
        private DataContext DataContext => Context as DataContext;
        public RecordRepository(DataContext dataDontext): base(dataDontext) {}

        public IEnumerable<RecordEntity> Local()
        {
            return DataContext.Records.Local.ToList();
        }
    }
}