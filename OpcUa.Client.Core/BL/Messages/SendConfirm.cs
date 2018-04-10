namespace OpcUa.Client.Core
{
    public class SendConfirm
    {
        public NotificationMessageViewModel Notification { get; set; }

        public SendConfirm(NotificationMessageViewModel notification)
        {
            Notification = notification;
        }
    }
}
