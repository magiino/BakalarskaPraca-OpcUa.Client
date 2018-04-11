using System;

namespace OpcUa.Client.Core
{
    public class SendNotificationAdd
    {
        public string Name { get; set; }
        public string NodeId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public SendNotificationAdd(string name, string nodeId,string message, DateTime time)
        {
            Name = name;
            NodeId = nodeId;
            Message = message;
            Time = time;
        }
    }
}
