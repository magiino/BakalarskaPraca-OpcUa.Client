using System.Security;

namespace OpcUa.Client.Core
{
    public class UserModel
    {
        public string UserName { get; set; }
        public SecureString Password { get; set; }
    }
}
