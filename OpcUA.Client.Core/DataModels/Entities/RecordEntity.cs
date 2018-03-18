using System.Collections.Generic;

namespace OpcUA.Client.Core
{
    public class RecordEntity : BaseEntity
    {
        public int VariableEntityID { get; set; }
        public VariableEntity Variable { get; set; }
        public string Value { get; set; }
        public virtual ICollection<RecordTimeEntity> Records { get; set; } = new List<RecordTimeEntity>();
    }
}
