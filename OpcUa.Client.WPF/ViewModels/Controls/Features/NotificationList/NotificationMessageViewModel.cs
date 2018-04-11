using System;
using System.Windows.Input;
using System.Windows.Media;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class NotificationMessageViewModel : BaseViewModel
    {
        public Guid Guid = Guid.NewGuid();
        public string Name { get; set; }
        public string NodeId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public ICommand ConfirmCommand { get; set; }

        public Brush NotificationColor
        {
            get
            {
                var r = new Random();
                return new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255),
                    (byte)r.Next(1, 255), (byte)r.Next(1, 233)));
            }
        }
           


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
