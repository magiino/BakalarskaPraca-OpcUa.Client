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
        private ReferenceDescription _refDescOfSelectedNode;
        private Subscription _subscription; 

        #endregion

        #region Public Properties

        public ObservableCollection<VariableModel> Notifications { get; set; } = new ObservableCollection<VariableModel>();
        public VariableModel SelectedNotification { get; set; }

        public bool AddIsEnabled { get; set; }
        public bool DeleteIsEnabled => SelectedNotification != null;

        #endregion

        #region Commands

        public ICommand AddNotificationCommand { get; set; }
        public ICommand RemoveNotificationCommand { get; set; }

        #endregion

        #region Constructor

        // TODO prerobit _selectedNode z refDisc na NodeId
        // TODO Prerobit WriteValue v opcuaApi
        // TODO Stale tam zobrazovat atributy len menit hodnoty !!!
        public NotificationViewModel(UaClientApi uaClientApi, DataContext dataContext)
        {
            _uaClientApi = uaClientApi;
            _dataContext = dataContext;
            _subscription = _uaClientApi.NotificationSubscription();

            //AddNotificationCommand = new RelayCommand(AddNotification);
            //RemoveNotificationCommand = new RelayCommand(DeleteNotification);
            //AddNotificationCommand = new Rela

            MessengerInstance.Register<SendSelectedRefNode>(
                this,
                node =>
                {
                    _refDescOfSelectedNode = node.RefNode;
                    AddIsEnabled = _refDescOfSelectedNode.NodeClass == NodeClass.Variable;
                });

            MessengerInstance.Register<SendMonitoredItem>(
                this,
                item => AddNotificationToSubscription(item.Item));
        }

        #endregion

        #region Command Methods

        private void AddNotification()
        {
            if (_refDescOfSelectedNode.NodeClass != NodeClass.Variable)
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
                NodeId = _refDescOfSelectedNode.NodeId.ToString()
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

        private void DeleteNotification()
        {
            if (SelectedNotification == null) return;
            _uaClientApi.RemoveMonitoredItem(_subscription, SelectedSubscribedVariableModel.NodeId);
            Notifications.Remove(SelectedSubscribedVariableModel);
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

            SubscriptionCreated = true;
        }

        private void WriteValue()
        {
            _uaClientApi.WriteValue(SelectedSubscribedVariableModel, ValueToWrite);
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

            //_dataContext.Records.Add(new RecordEntity()
            //{
            //    ArchiveTime = value.ServerTimestamp,
            //});

            variable.Value = value.Value;
            variable.StatusCode = value.StatusCode;
            variable.ServerDateTime = value.ServerTimestamp;
            variable.SourceDateTime = value.SourceTimestamp;
        }

        #endregion
    }
}