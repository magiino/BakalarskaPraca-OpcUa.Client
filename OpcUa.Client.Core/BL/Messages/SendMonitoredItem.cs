using Opc.Ua.Client;

namespace OpcUa.Client.Core
{
    public class SendMonitoredItem
    {
        public MonitoredItem Item { get; set; }

        public SendMonitoredItem(MonitoredItem item)
        {
            Item = item;
        }
    }
}
