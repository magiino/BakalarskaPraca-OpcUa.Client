using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class NotificationListViewModel : BaseViewModel
    { 
        public ObservableCollection<NotificationMessageViewModel> Items { get; set; } = new ObservableCollection<NotificationMessageViewModel>();

        public NotificationListViewModel()
        {
            IoC.Messenger.Register<SendNotificationDelete>((msg) =>
            {
                var itemToDelete = Items.SingleOrDefault(x => x.Guid == msg.Identifier);
                Items.Remove(itemToDelete);
            });

            IoC.Messenger.Register<SendNotificationAdd>((msg) =>
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    Items.Add(new NotificationMessageViewModel()
                    {
                        Message = msg.Message,
                        Name = msg.Name,
                        NodeId = msg.NodeId,
                        Time = msg.Time,
                    });
                });

            });
        }
    }
}
