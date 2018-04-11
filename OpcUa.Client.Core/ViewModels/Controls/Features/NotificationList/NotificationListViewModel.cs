using System;
using System.Collections.ObjectModel;

namespace OpcUa.Client.Core
{
    public class NotificationListViewModel : BaseViewModel
    {
        public ObservableCollection<NotificationMessageViewModel> Items { get; set; } = new ObservableCollection<NotificationMessageViewModel>();

        public NotificationListViewModel()
        {
            MessengerInstance.Register<SendNotificationDelete>((msg) => Items.Remove(msg.Notification));

            

            MessengerInstance.Register<SendNotificationAdd>((msg) =>
            {
                //App.Current.Dispatcher.Invoke((Action)delegate
                //{
                //    Items.Add(msg.Notification);
                //});
                
            });
        }
    }
}
