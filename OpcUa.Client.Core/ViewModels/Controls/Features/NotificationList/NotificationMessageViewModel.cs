using System;
using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class NotificationMessageViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public NotificationMessageViewModel(ExtendedNotificationModel notification)
        {
            if (notification.IsDigital && (bool)notification.Value)
                Message = notification.IsOneDescription;
            else if (notification.IsDigital && !(bool)notification.Value)
                Message = notification.IsZeroDescription;
            else if (!notification.IsDigital)
                Message = $"Hodnota premennej {notification.Name} sa zmenila o {notification.Value} {notification.DeadbandType.ToString()}";

            Name = notification.Name;
            Time = notification.SourceDateTime;

            ConfirmCommand = new RelayCommand(SendConfirm);
        }

        private void SendConfirm()
        {
            MessengerInstance.Send(new SendConfirm(this));
        }
    }
}
