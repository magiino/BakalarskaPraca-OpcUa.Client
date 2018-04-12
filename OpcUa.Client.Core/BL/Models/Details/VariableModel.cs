using System;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class VariableModel
    {
        public int Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public BuiltInType DataType { get; set; }
        public ArchiveInterval Archive { get; set; }
    }
}