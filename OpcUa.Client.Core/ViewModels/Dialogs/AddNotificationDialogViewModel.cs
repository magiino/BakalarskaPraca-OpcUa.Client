using System;
using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class AddNotificationDialogViewModel : BaseDialogViewModel
    {
        public string DisplayName { get; set; }
        public string NodeId { get; set; }
        public string Description { get; set; }
        public string FilterValue { get; set; } = "0";

        public ICommand AddItemCommand { get; set; }

        public AddNotificationDialogViewModel()
        {
            AddItemCommand = new RelayCommand(AddItem);
        }

        private void AddItem()
        {
            var item = IoC.UaClientApi.NotificationMonitoredItem(DisplayName, NodeId, Convert.ToDouble(FilterValue));
            MessengerInstance.Send(new SendMonitoredItem(item));
            CloseAction.Invoke();
        }
    }
}
