using System.Collections.Generic;

namespace OpcUa.Client.Core
{
    public class NotificationListViewModel : BaseViewModel
    {
        public List<NotificationMessageViewModel> Items { get; set; } = new List<NotificationMessageViewModel>();

        public NotificationListViewModel()
        {
            MessengerInstance.Register<SendConfirm>((msg) => Items.Remove(msg.Notification));
        }
    }
}
