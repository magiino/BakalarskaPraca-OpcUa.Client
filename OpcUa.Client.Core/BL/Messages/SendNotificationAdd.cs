namespace OpcUa.Client.Core
{
    public class SendNotificationAdd
    {
        public NotificationMessageViewModel Notification { get; set; }

        public SendNotificationAdd(NotificationMessageViewModel notificationToAdd)
        {
            Notification = notificationToAdd;
        }
    }
}
