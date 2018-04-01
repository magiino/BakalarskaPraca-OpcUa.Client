using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUa.Client.Core
{
    public class NotificationViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;
        private readonly DataContext _dataContext;
        private ReferenceDescription _selectedNode;
        private Subscription _subscription; 

        #endregion

        #region Public Properties

        public ObservableCollection<VariableModel> Notifications { get; set; } = new ObservableCollection<VariableModel>();
        public VariableModel SelectedNotification { get; set; }

        #endregion

        #region Commands

        public ICommand AddNotificationCommand { get; set; }
        public ICommand RemoveNotificationCommand { get; set; }

        #endregion

        #region Constructor

        // TODO Prerobit WriteValue v opcuaApi
        public NotificationViewModel(UaClientApi uaClientApi, DataContext dataContext)
        {
            _uaClientApi = uaClientApi;
            _dataContext = dataContext;
            _subscription = _uaClientApi.NotificationSubscription();

            //AddNotificationCommand = new RelayCommand(AddNotification);
            AddNotificationCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(AddNotification, AddNotificationCanUse);
            //RemoveNotificationCommand = new RelayCommand(RemoveNotification);
            RemoveNotificationCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(RemoveNotification, RemoveNotificationCanUse);

            MessengerInstance.Register<SendSelectedRefNode>(
                this,
                msg => _selectedNode = msg.ReferenceNode);

            MessengerInstance.Register<SendMonitoredItem>(
                this,
                msg => AddNotificationToSubscription(msg.Item));
        }

        #endregion

        #region Command Methods

        private void AddNotification()
        {
            if (_selectedNode.NodeClass != NodeClass.Variable)
            {
                IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
                {
                    Title = "Error",
                    Message = "Musíte zvoliť Nodu typu Variable!",
                    OkText = "Ok"
                });
                return;
            }

            IoC.Ui.ShowAddNotification(new AddNotificationDialogViewModel()
            {
                NodeId = _selectedNode.NodeId.ToString()
            });
        }

        private void AddNotificationToSubscription(MonitoredItem item)
        {
            var nodeId = item.StartNodeId.ToString();
            var type = _uaClientApi.GetBuiltInTypeOfVariableNodeId(nodeId);

            // TODO private metoda opakovany kod
            var tmp = new VariableModel()
            {
                NodeId = nodeId,
                Name = item.DisplayName,
                DataType = type,
                Value = "No change yet",
            };

            _uaClientApi.AddMonitoredItem(item, _subscription);
            item.Notification += Notification_MonitoredItem;

            Notifications.Add(tmp);
        }

        private void RemoveNotification()
        {
            if (SelectedNotification == null) return;
            _uaClientApi.RemoveMonitoredItem(_subscription, SelectedNotification.NodeId);
            Notifications.Remove(SelectedNotification);
        }

        private void SaveSubscription()
        {
            // TODO save do priecinka /Subscriptions a ukladat s mneom ako datum v stringu
            _uaClientApi.SaveSubsciption();
        }

        private void LoadSubscription()
        {
            // TODO opytat sa uzivatela ci chce prepisat existujucu subscription
            if (_subscription != null) return;
            _subscription = _uaClientApi.LoadSubsciption();

            if (_subscription == null) return;

            // TODO private metoda opakovany kod
            foreach (var item in _subscription.MonitoredItems)
            {
                var tmp = new VariableModel()
                {
                    NodeId = item.StartNodeId.ToString(),
                    Name = item.DisplayName
                    // TODO set up type here
                };

                item.Notification += Notification_MonitoredItem;
                Notifications.Add(tmp);
            }
            _subscription.ApplyChanges();

        }

        #endregion

        #region Can use methods

        public bool AddNotificationCanUse()
        {
            if (_selectedNode == null)
                return false;
            else if (_selectedNode.NodeClass != NodeClass.Variable)
                return false;
            else return true;
        }

        public bool RemoveNotificationCanUse()
        {
            return SelectedNotification != null;
        }

        #endregion

        #region CallBack Methods

        /// <summary>
        /// Callback method for updating values of subscibed nodes
        /// </summary>
        /// <param name="monitoredItem"></param>
        /// <param name="e"></param>
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (!(e.NotificationValue is MonitoredItemNotification notification))
                return;

            var value = notification.Value;

            var variable = Notifications.FirstOrDefault(x => x.Name == monitoredItem.DisplayName);

            if (variable == null) return;

            variable.Value = value.Value;
            variable.StatusCode = value.StatusCode;
            variable.ServerDateTime = value.ServerTimestamp;
            variable.SourceDateTime = value.SourceTimestamp;
        }

        #endregion
    }
}