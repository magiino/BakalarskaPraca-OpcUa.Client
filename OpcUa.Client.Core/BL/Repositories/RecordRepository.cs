namespace OpcUa.Client.Core
{
    public class RecordRepository : BaseRepository<RecordEntity>, IRecordRepository
    {
        private DataContext DataContext => Context as DataContext;
        public RecordRepository(DataContext dataDontext): base(dataDontext) {}

        public void DeleteAllWithVariableId(int id)
        {
            
        }
    }
}