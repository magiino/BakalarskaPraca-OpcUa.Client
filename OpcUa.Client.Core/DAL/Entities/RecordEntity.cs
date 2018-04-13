using System;

namespace OpcUa.Client.Core
{
    public class RecordEntity : BaseEntity
    {
        public int VariableId { get; set; }
        public VariableEntity Variable { get; set; }
        public string Value { get; set; }
        public DateTime ArchiveTime { get; set; }
    }
}
