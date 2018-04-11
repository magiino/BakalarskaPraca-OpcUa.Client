using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpcUa.Client.Core
{
    public class ProjectEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        public int EndpointId { get; set; }
        [ForeignKey("EndpointId")]
        public EndpointEntity Endpoint { get; set; }
        [Required]
        public string SessionName { get; set; }
        public int? UserId { get; set; }
        public UserEntity User { get; set; }
    }
}