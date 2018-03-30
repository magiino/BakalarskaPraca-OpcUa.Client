using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUa.Client.Core
{
    public class SubscriptionViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;
        private readonly DataContext _dataContext;
        private ReferenceDescription _refDescOfSelectedNode;
        private Subscription _subscription; 

        #endregion

        #region Public Properties

        public ObservableCollection<VariableModel> SubscribedVariables { get; set; } = new ObservableCollection<VariableModel>();
        public VariableModel SelectedSubscribedVariableModel { get; set; }
        public string ValueToWrite { get; set; }

        public bool SubscriptionCreated { get; set; }
        public bool AddIsEnabled { get; set; }
        public bool DeleteIsEnabled => SelectedSubscribedVariableModel != null;

        #endregion

        #region Commands

        public ICommand AddVariableToSubscriptionCommand { get; set; }
        public ICommand DeleteVariableFromSubscriptionCommand { get; set; }
        public ICommand CreateSubscriptionCommand { get; set; }
        public ICommand DeleteSubscriptionCommand { get; set; }
        public ICommand LoadSubscriptionCommand { get; set; }
        public ICommand SaveSubscriptionCommand { get; set; }
        public ICommand WriteValueCommand { get; set; }

        #endregion

        #region Constructor

        // TODO prerobit _selectedNode z refDisc na NodeId
        // TODO Prerobit WriteValue v opcuaApi
        // TODO Stale tam zobrazovat atributy len menit hodnoty !!!
        public SubscriptionViewModel(UaClientApi uaClientApi, DataContext dataContext)
        {
            _uaClientApi = uaClientApi;
            _dataContext = dataContext;

            AddVariableToSubscriptionCommand = new RelayCommand(AddVariableToSubscription);
            DeleteVariableFromSubscriptionCommand = new RelayCommand(DeleteVariableFromSubscription);
            CreateSubscriptionCommand = new RelayCommand(CreateSubscription);
            DeleteSubscriptionCommand = new RelayCommand(DeleteSubscrition);
            SaveSubscriptionCommand = new RelayCommand(SaveSubscription);
            LoadSubscriptionCommand = new RelayCommand(LoadSubscription);
            WriteValueCommand = new RelayCommand(WriteValue);

            MessengerInstance.Register<SendSelectedRefNode>(
                this,
                node =>
                {
                    _refDescOfSelectedNode = node.RefNode;
                    AddIsEnabled = _refDescOfSelectedNode.NodeClass == NodeClass.Variable;
                });
        }

        #endregion

        #region Command Methods

        private void CreateSubscription()
        {
            _subscription = _uaClientApi.Subscribe(2000);
            if (_subscription == null) return;
            SubscriptionCreated = true;
        }

        private void DeleteSubscrition()
        {
            _uaClientApi.RemoveSubscription(_subscription);
            _subscription = null;
            SubscribedVariables.Clear();
            SubscriptionCreated = false;
            //_subscription.MonitoredItemCount;
            //_subscription.Created;
            //_subscription.DisplayName;
            //_subscription.Id;
        }

        private void AddVariableToSubscription()
        {
            if (_refDescOfSelectedNode == null) return;
            // TODO private metoda opakovany kod
            var tmp = new VariableModel()
            {
                NodeId = _refDescOfSelectedNode.NodeId.ToString(),
                Name = _refDescOfSelectedNode.DisplayName.ToString()
                // TODO set up type here
            };

            var monitoredItem = _uaClientApi.AddMonitoredItem(_refDescOfSelectedNode, _subscription);
            monitoredItem.Notification += Notification_MonitoredItem;

            SubscribedVariables.Add(tmp);
        }

        private void DeleteVariableFromSubscription()
        {
            if (SelectedSubscribedVariableModel == null) return;
            _uaClientApi.RemoveMonitoredItem(_subscription, SelectedSubscribedVariableModel.NodeId);
            SubscribedVariables.Remove(SelectedSubscribedVariableModel);
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
                SubscribedVariables.Add(tmp);
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

            var variable = SubscribedVariables.FirstOrDefault(x => x.Name == monitoredItem.DisplayName);

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