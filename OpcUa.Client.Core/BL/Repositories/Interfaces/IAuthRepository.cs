using System.Security;

namespace OpcUa.Client.Core
{
    public interface IAuthRepository
    {
        UserEntity Register(UserEntity user, SecureString password);
        UserEntity Login(string userName, SecureString password);
        bool UserExists(string userName);
        void RemoveUser(int id);
    }
}
