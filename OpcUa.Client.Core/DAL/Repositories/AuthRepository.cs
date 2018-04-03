using System.Linq;
using System.Security;

namespace OpcUa.Client.Core
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public UserEntity Register(UserEntity user, SecureString password)
        {
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

             _context.Users.Add(user);
            return user;
        }

        public UserEntity Login(string userName, SecureString password)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == userName);

            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;

        }

        public bool UserExists(string userName)
        {
            return _context.Users.Any(x => x.UserName == userName);
        }

        private void CreatePasswordHash(SecureString password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(SecureStringHelpers.Unsecure(password)));
            }
        }

        private bool VerifyPasswordHash(SecureString password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(SecureStringHelpers.Unsecure(password)));
                if (computedHash.Where((t, i) => t != passwordHash[i]).Any())
                    return false;
            }
            return true;
        }
    }
}