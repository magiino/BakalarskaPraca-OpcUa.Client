namespace OpcUa.Client.Core
{
    public class VariableRepository : BaseRepository<VariableEntity>, IVariableRepository
    {
        private DataContext DataContect => Context as DataContext;
        public VariableRepository(DataContext dataDontext): base(dataDontext) {}
    }
}