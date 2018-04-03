using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class AnalogNotificationModel //: ExtendedNotificationModel
    {
        public double FilterValue { get; set; }
        public DeadbandType DeadbandType { get; set; }
    }
}
