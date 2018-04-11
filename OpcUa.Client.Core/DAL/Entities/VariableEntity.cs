using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class VariableEntity : BaseEntity
    {
        [Required]
        public Guid ProjectId { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public BuiltInType DataType { get; set; }
        public ArchiveInterval Archive { get; set; }
        public virtual ICollection<RecordEntity> Records { get; set; } = new List<RecordEntity>();
    }
}