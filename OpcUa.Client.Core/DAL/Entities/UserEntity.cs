using System.ComponentModel.DataAnnotations;

namespace OpcUa.Client.Core
{
    public class UserEntity : BaseEntity
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
    }
}