using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class NotificationEntity : BaseEntity
    {
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public string NodeId { get; set; }
        public double FilterValue { get; set; }
        public DeadbandType DeadbandType { get; set; }
        public bool IsDigital { get; set; }
        [MaxLength(50)]
        public string IsZeroDescription { get; set; }
        [MaxLength(50)]
        public string IsOneDescription { get; set; }
    }
}