using System;
using System.Collections.Generic;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class VariableEntity : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BuiltInType DataType { get; set; }
        public ArchiveInterval Archive { get; set; }
        public virtual ICollection<RecordEntity> Records { get; set; } = new List<RecordEntity>();
    }
}