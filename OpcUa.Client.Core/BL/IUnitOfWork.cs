using System;

namespace OpcUa.Client.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }
        IVariableRepository Variables { get; }
        IRecordRepository Records { get; }
        IAuthRepository Auth { get; }
        int Complete();
    }
}