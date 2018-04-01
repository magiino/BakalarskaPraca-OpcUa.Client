namespace OpcUa.Client.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        public UnitOfWork(DataContext context)
        {
            _dataContext = context;
            Projects = new ProjectRepository(_dataContext);
            Variables = new VariableRepository(_dataContext);
            Records = new RecordRepository(_dataContext);
            Auth = new AuthRepository(_dataContext);
        }
        public IProjectRepository Projects { get; private set; }
        public IVariableRepository Variables { get; private set; }
        public IRecordRepository Records { get; private set; }
        public IAuthRepository Auth { get; private set; }

        public int Complete()
        {
            return _dataContext.SaveChanges();
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }
    }
}