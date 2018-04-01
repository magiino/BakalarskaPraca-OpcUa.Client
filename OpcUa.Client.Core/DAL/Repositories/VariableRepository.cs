namespace OpcUa.Client.Core
{
    public class VariableRepository : BaseRepository<VariableEntity>, IVariableRepository
    {
        public VariableRepository(DataContext dataDontext): base(dataDontext) {}
    }
}