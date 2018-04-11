namespace OpcUa.Client.Core
{
    public class SendNotificationDelete
    {
        public NotificationMessageViewModel Notification { get; set; }

        public SendNotificationDelete(NotificationMessageViewModel notificationToDelete)
        {
            Notification = notificationToDelete;
        }
    }
}
