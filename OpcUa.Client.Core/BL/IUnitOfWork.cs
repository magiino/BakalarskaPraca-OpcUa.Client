using System;

namespace OpcUa.Client.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }
        IVariableRepository Variables { get; }
        IRecordRepository Records { get; }
        INotificationRepository Notifications { get; }
        IEndpointRepository Endpoints { get; }
        IAuthRepository Auth { get; }
        int Complete();
        bool HasUnsavedChanges();
    }
}