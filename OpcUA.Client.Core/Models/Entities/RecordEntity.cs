using System;

namespace OpcUA.Client.Core
{
    public class RecordEntity : BaseEntity
    {
        public int VariableEntityID { get; set; }
        public VariableEntity Variable { get; set; }
        public string Value { get; set; }
        public DateTime ArchiveTime { get; set; }
    }
}
