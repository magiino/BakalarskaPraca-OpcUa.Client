using System;

namespace OpcUa.Client.Core
{
    public class SendNotificationAdd
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public SendNotificationAdd(string name, string message, DateTime time)
        {
            Name = name;
            Message = message;
            Time = time;
        }
    }
}
