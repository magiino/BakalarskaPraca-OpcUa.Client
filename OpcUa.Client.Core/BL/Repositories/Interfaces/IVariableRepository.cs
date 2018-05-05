namespace OpcUa.Client.Core
{
    public interface IVariableRepository : IBaseRepository<VariableEntity>
    {
        void DeleteById(int id);
    }
}