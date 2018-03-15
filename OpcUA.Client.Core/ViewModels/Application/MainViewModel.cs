using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUA.Client.Core
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;

        private Subscription _subscription;

        private ReferenceDescription _refDiscOfSelectedNode;

        #endregion

        #region Public Properties

        public NodeTreeViewModel NodetreeViewModel { get; set; }

        public NodeAttributesViewModel NodeAttributesViewModel { get; set; }

        public ObservableCollection<AttributeDataGridModel> SelectedNode { get; set; } = new ObservableCollection<AttributeDataGridModel>();

        public bool AddIsEnabled => SelectedNode.Count != 0;


        public ObservableCollection<Variable> SubscribedVariables { get; set; } = new ObservableCollection<Variable>();

        #endregion

        #region Commands

            #region MenuBar Commands

            public ICommand NewSessionCommand { get; set; }
            public ICommand SaveSessionCommand { get; set; }
            public ICommand OpenSessionCommand { get; set; }
            public ICommand ExitApplicationCommand { get; set; }

            #endregion

            #region ToolBar Commands

            public ICommand ConnectSessionCommand { get; set; }
            public ICommand DisconnectSessionCommand { get; set; }

            #endregion

        public ICommand AddVariableToSubscriptionCommand { get; set; }

        public ICommand CreateSubscriptionCommand { get; set; }

        public ICommand DeleteSubscriptionCommand { get; set; }
        public ICommand LoadSubscriptionCommand { get; set; }
        public ICommand SaveSubscriptionCommand { get; set; }
        public ICommand DeleteVariableFromSubscriptionCommand { get; set; }

        public ICommand WriteValueCommand { get; set; }

        public Variable SelectedSubscribedVariable { get; set; }

        public bool DeleteIsEnabled => SelectedSubscribedVariable != null;

        public bool SubscriptionCreated { get; set; }

        public string ValueToWrite { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            _uaClientApi = IoC.UaClientApi;

            NewSessionCommand = new RelayCommand(NewSession);
            SaveSessionCommand = new RelayCommand(SaveSession);
            OpenSessionCommand = new RelayCommand(OpenSession);
            ExitApplicationCommand = new RelayCommand(ExitApplication);
            ConnectSessionCommand = new RelayCommand(ConnectSession);
            DisconnectSessionCommand = new RelayCommand(DisconnectSession);

            AddVariableToSubscriptionCommand = new RelayCommand(AddVariableToSubscription);
            DeleteVariableFromSubscriptionCommand = new RelayCommand(DeleteVariableFromSubscription);
            CreateSubscriptionCommand = new RelayCommand(CreateSubscription);
            DeleteSubscriptionCommand = new RelayCommand(DeleteSubscrition);
            SaveSubscriptionCommand = new RelayCommand(SaveSubscription);
            LoadSubscriptionCommand = new RelayCommand(LoadSubscription);
            WriteValueCommand = new RelayCommand(WriteValue);

            NodetreeViewModel = new NodeTreeViewModel(SetSelectedNode);
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
            SubscribedVariables.Clear();
            SubscriptionCreated = false;
        }

        private void AddVariableToSubscription()
        {
            if (_refDiscOfSelectedNode == null) return;
            var tmp = new Variable()
            {
                MonitoredItem = _uaClientApi.AddMonitoredItem(_refDiscOfSelectedNode, _subscription),
            };

            tmp.MonitoredItem.Notification += Notification_MonitoredItem;
            SubscribedVariables.Add(tmp);
        }

        private void DeleteVariableFromSubscription()
        {
            if(SelectedSubscribedVariable == null) return;
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
            _subscription = _uaClientApi.LoadSubsciption().FirstOrDefault();
        }

        private void WriteValue()
        {
            _uaClientApi.WriteValue(SelectedSubscribedVariable, ValueToWrite);
        }



        private void NewSession() {}
        private void SaveSession()
        {
            _uaClientApi.SaveConfiguration();
        }
        private void OpenSession() {}
        private void ExitApplication() {}
        private void ConnectSession() {}
        private void DisconnectSession() {}

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

        /// <summary>
        /// Callback method for set up selected node
        /// </summary>
        /// <param name="selectedNode"></param>
        private void SetSelectedNode(ReferenceDescription selectedNode)
        {
            _refDiscOfSelectedNode = selectedNode;
            SelectedNode = new ObservableCollection<AttributeDataGridModel>(GetDataGridModel(selectedNode));
        } 
        #endregion

        #region Private Helpers

        /// <summary>
        /// Takes attributes and values of <see cref="ReferenceDescription"/> object
        /// </summary>
        /// <param name="referenceDescription"></param>
        /// <returns></returns>
        private ObservableCollection<AttributeDataGridModel> GetDataGridModel(ReferenceDescription referenceDescription)
        {
            var data = new ObservableCollection<AttributeDataGridModel>();

            foreach (var propertyInfo in referenceDescription.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(referenceDescription);

                if (value is NodeId)
                    value.GetType().GetProperties().ToList().ForEach(property => data.Add(new AttributeDataGridModel(property.Name, property.GetValue(value).ToString())));

                data.Add(new AttributeDataGridModel(propertyInfo.Name, value.ToString()));
            }

            var node = _uaClientApi.ReadNode(referenceDescription.NodeId.ToString());
            node.GetType().GetProperties().ToList().ForEach(property => data.Add(new AttributeDataGridModel(property.Name, property.GetValue(node)?.ToString())));

            if (node.NodeClass != NodeClass.Variable) return data;

            var variableNode = (VariableNode) node.DataLock;
            variableNode.GetType().GetProperties().ToList().ForEach(property =>
            data.Add(new AttributeDataGridModel(property.Name, property.GetValue(variableNode)?.ToString())));

            return data;
        }
        #endregion
    }
}

//SubscribedVariables.CollectionChanged += (s, e) =>
//{
//if (SubscribedVariables.Count == 1 && e.Action == NotifyCollectionChangedAction.Add)
//{
//Task.Run(() =>
//{
//    while (true)
//    {
//        _uaClientApi.ReadValues(SubscribedVariables.Select(x => x.NodeId.ToString()).ToList());
//    }
//});
//}
//};