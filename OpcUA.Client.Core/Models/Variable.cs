using System;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class Variable : BaseViewModel
    {
        public string NodeId { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        public Type DataType => Value?.GetType();

        public StatusCode StatusCode { get; set; }

        public DateTime DateTime { get; set; }
    }
}
