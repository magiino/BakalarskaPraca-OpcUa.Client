using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUA.Client.Core
{
    public class SubscriptionViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _refDiscOfSelectedNode;
        private Subscription _subscription; 

        #endregion

        #region Public Properties

        public ObservableCollection<Variable> SubscribedVariables { get; set; } = new ObservableCollection<Variable>();
        public Variable SelectedSubscribedVariable { get; set; }
        public string ValueToWrite { get; set; }
        public bool DeleteIsEnabled => SelectedSubscribedVariable != null;
        public bool SubscriptionCreated { get; set; }
        //public bool AddIsEnabled => SelectedNode.Count != 0;

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
        // TODO vracat z write value ci sa podaril zapis
        // TODO Stale tam zobrazovat atributy len menit hodnoty !!!
        // TODO Urobit ViewModel zvlast pre vsetko
        public SubscriptionViewModel(UaClientApi uaClientApi)
        {
            _uaClientApi = uaClientApi;

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
                    _refDiscOfSelectedNode = node.RefNode;
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
            if (_refDiscOfSelectedNode == null) return;
            // TODO private metoda opakovany kod
            var tmp = new Variable()
            {
                MonitoredItem = _uaClientApi.AddMonitoredItem(_refDiscOfSelectedNode, _subscription),
                // TODO set up type here
            };

            tmp.MonitoredItem.Notification += Notification_MonitoredItem;
            SubscribedVariables.Add(tmp);
        }

        private void DeleteVariableFromSubscription()
        {
            if (SelectedSubscribedVariable == null) return;
            _uaClientApi.RemoveMonitoredItem(_subscription, SelectedSubscribedVariable.MonitoredItem);
            SubscribedVariables.Remove(SelectedSubscribedVariable);
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
                var tmp = new Variable()
                {
                    MonitoredItem = item,
                    // TODO set up type here
                };

                tmp.MonitoredItem.Notification += Notification_MonitoredItem;
                SubscribedVariables.Add(tmp);
            }
            _subscription.ApplyChanges();

            SubscriptionCreated = true;
        }

        private void WriteValue()
        {
            _uaClientApi.WriteValue(SelectedSubscribedVariable, ValueToWrite);
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
            variable.Value = value.Value;
            variable.StatusCode = value.StatusCode;
            variable.DateTime = value.ServerTimestamp;
        }

        #endregion
    }
}
