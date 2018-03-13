using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUA.Client.Core
{
    public class MainViewModel : BaseViewModel
    {
        #region PrivateFields

        private readonly UaClientApi _uaClientApi;

        #endregion

        #region Public Properties

        public NodeTreeViewModel NodetreeViewModel { get; set; }

        public NodeAttributesViewModel NodeAttributesViewModel { get; set; }

        private ReferenceDescription _refDiscOfSelectedNode;

        /// <summary>
        /// A list of attributes and values of <see cref="ReferenceDescription"/> object
        /// </summary>
        public ObservableCollection<AttributeDataGridModel> SelectedNode { get; set; } = new ObservableCollection<AttributeDataGridModel>();

        /// <summary>
        /// Allowing to add variable to list only if node is selected
        /// </summary>
        public bool AddIsEnabled => SelectedNode != null;

        public ObservableCollection<Variable> VariablesToRead { get; set; } = new ObservableCollection<Variable>();

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
            CreateSubscriptionCommand = new RelayCommand(CreateSubscription);

            NodetreeViewModel = new NodeTreeViewModel(SetSelectedNode);

        }

        #endregion

        #region Command Methods

        private void CreateSubscription()
        {
            _uaClientApi.Subscribe(2000);
            //_uaClientApi.ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
        }

        private void AddVariableToSubscription()
        {
            var nodeId = new NodeId(_refDiscOfSelectedNode.NodeId.ToString());
            VariablesToRead.Add(new Variable()
            {
                NodeId = nodeId,
                DataType = _uaClientApi.GetDataType(nodeId).ToString()
            });

            _uaClientApi.AddMonitoredItem(nodeId, 1).Notification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
        }

        private void NewSession()
        {
            IoC.Application.GoToPage(ApplicationPage.Welcome);
        }

        private void SaveSession() {}

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
            var notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null)
            {
                return;
            }
            VariablesToRead.First(x => x.Name == monitoredItem.DisplayName).Value = notification.Value.Value;
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

            var node = IoC.UaClientApi.ReadNode(referenceDescription.NodeId.ToString());
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




//VariablesToRead.CollectionChanged += (s, e) =>
//{
//if (VariablesToRead.Count == 1 && e.Action == NotifyCollectionChangedAction.Add)
//{
//Task.Run(() =>
//{
//    while (true)
//    {
//        _uaClientApi.ReadValues(VariablesToRead.Select(x => x.NodeId.ToString()).ToList());
//    }
//});
//}
//};