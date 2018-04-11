using System.Security;

namespace OpcUa.Client.Core
{
    public class SendCredentials
    {
        public string UserName { get; set; }
        public SecureString Password { get; set; }

        public SendCredentials(string userName, SecureString password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
