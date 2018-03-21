using System;

namespace OpcUA.Client.Core
{
    public class ChartModel<T> : BaseViewModel
    {
        public DateTime DateTime { get; set; }
        public T Value { get; set; }
    }
}
