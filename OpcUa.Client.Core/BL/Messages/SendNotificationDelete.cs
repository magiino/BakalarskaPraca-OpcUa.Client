using System;

namespace OpcUa.Client.Core
{
    public class SendNotificationDelete
    {
        public Guid Identifier { get; set; }

        public SendNotificationDelete(Guid identifier)
        {
            Identifier = identifier;
        }
    }
}
