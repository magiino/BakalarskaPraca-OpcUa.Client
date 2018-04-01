namespace OpcUa.Client.Core
{
    public class RecordRepository : BaseRepository<RecordEntity>, IRecordRepository
    {
        public RecordRepository(DataContext dataDontext): base(dataDontext) {}
    }
}