using System;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUa.Client.Core
{
    public class AddNotificationDialogViewModel : BaseDialogViewModel
    {
        #region Private Fields
        private readonly Messenger _messenger; 
        #endregion

        #region Public Properties
        public string Name { get; set; }
        public string NodeId { get; set; }
        public bool IsDigital { get; set; }
        public string FilterValue { get; set; } = "0";
        private DeadbandType _selectedFilterType;
        public int SelectedFilterType
        {
            get => 0;
            set => _selectedFilterType = (DeadbandType)(value);
        }

        public string IsZeroDescription { get; set; }
        public string IsOneDescription { get; set; }
        #endregion

        #region Commands
        public ICommand AddItemCommand { get; } 
        #endregion

        #region Constructor
        public AddNotificationDialogViewModel(Messenger messenger)
        {
            _messenger = messenger;
            AddItemCommand = new RelayCommand(AddItem);
        }
        #endregion

        #region Command Methods
        private void AddItem(object parameter)
        {
            var notification = new ExtendedNotificationModel();

            if (IsDigital)
            {
                notification.IsZeroDescription = IsZeroDescription;
                notification.IsOneDescription = IsOneDescription;
                notification.IsDigital = true;
            }
            else
            {
                notification.FilterValue = Convert.ToDouble(FilterValue);
                notification.DeadbandType = _selectedFilterType;
            }

            notification.Name = Name;
            notification.NodeId = NodeId;

            _messenger.Send(new SendNewNotification(notification));
            CloseAction.Invoke();
        } 
        #endregion
    }
}