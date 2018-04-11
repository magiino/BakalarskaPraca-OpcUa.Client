using System;
using System.Windows.Input;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class NotificationMessageViewModel : BaseViewModel
    {
        public Guid Guid = Guid.NewGuid();
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public NotificationMessageViewModel()
        {
            ConfirmCommand = new RelayCommand(SendConfirm);
        }

        private void SendConfirm(object parameter)
        {
            IoC.Messenger.Send(new SendNotificationDelete(Guid));
        }
    }
}
