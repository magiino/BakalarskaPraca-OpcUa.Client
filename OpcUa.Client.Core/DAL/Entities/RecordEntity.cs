using System;
using System.ComponentModel.DataAnnotations;

namespace OpcUa.Client.Core
{
    public class RecordEntity : BaseEntity
    {
        public int VariableId { get; set; }
        public VariableEntity Variable { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public DateTime ArchiveTime { get; set; }
    }
}
