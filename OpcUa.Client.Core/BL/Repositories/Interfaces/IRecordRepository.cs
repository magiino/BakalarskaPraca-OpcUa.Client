namespace OpcUa.Client.Core
{
    public interface IRecordRepository : IBaseRepository<RecordEntity>
    {
        void DeleteAllWithVariableId(int id);
    }
}