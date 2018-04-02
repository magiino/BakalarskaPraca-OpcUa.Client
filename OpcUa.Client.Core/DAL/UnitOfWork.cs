namespace OpcUa.Client.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        public int ProjectId { get; set; }

        public UnitOfWork(DataContext context)
        {
            _dataContext = context;
            Projects = new ProjectRepository(_dataContext);
            Variables = new VariableRepository(_dataContext);
            Records = new RecordRepository(_dataContext);
            Notifications = new NotificationRepository(_dataContext);
            Auth = new AuthRepository(_dataContext);
            Endpoints = new EndpointRepository(_dataContext);
        }
        public IProjectRepository Projects { get; }
        public IVariableRepository Variables { get; }
        public IRecordRepository Records { get; private set; }
        public INotificationRepository Notifications { get; }
        public IEndpointRepository Endpoints { get; }
        public IAuthRepository Auth { get; }

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