using System.Collections.Generic;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class VariableEntity : BaseEntity
    {
        public string Name { get; set; }
        public BuiltInType DataType { get; set; }
        public ArchiveInterval Archive { get; set; }
        public virtual ICollection<RecordEntity> Records { get; set; } = new List<RecordEntity>();
    }
}