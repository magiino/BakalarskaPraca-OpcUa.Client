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

        public NotificationMessageViewModel()
        {
            ConfirmCommand = new RelayCommand(SendConfirm);
        }

        private void SendConfirm()
        {
            MessengerInstance.Send(new SendNotificationDelete(this));
        }
    }
}
