namespace OpcUa.Client.Core
{
    public class SendNewNotification
    {
        public ExtendedNotificationModel Notification { get; set; }

        public SendNewNotification(ExtendedNotificationModel notification)
        {
            Notification = notification;
        }
    }
}
