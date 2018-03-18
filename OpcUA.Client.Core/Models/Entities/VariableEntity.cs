using System.Collections.Generic;

namespace OpcUA.Client.Core
{
    public class VariableEntity : BaseEntity
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public virtual ICollection<RecordEntity> Records { get; set; } = new List<RecordEntity>();
    }
}