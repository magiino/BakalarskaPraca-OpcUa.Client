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

        private NodeIdCollection _nodeIdsToRead = new NodeIdCollection();

        #endregion

        #region Public Properties

        public NodeTreeViewModel NodetreeViewModel { get; set; }

        public NodeAttributesViewModel NodeAttributesViewModel { get; set; }

        /// <summary>
        /// Selected node from node tree view
        /// </summary>
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

        /// <summary>
        /// The command for creating new session
        /// </summary>
        public ICommand NewSessionCommand { get; set; }

        /// <summary>
        /// The command for saving session
        /// </summary>
        public ICommand SaveSessionCommand { get; set; }

        /// <summary>
        /// The command for opening exsisting session
        /// </summary>
        public ICommand OpenSessionCommand { get; set; }

        /// <summary>
        /// The command for exit application
        /// </summary>
        public ICommand ExitApplicationCommand { get; set; }

        /// <summary>
        /// The command for exit application
        /// </summary>
        public ICommand AddVariable { get; set; }

        #endregion

        #region ToolBar Commands

        /// <summary>
        /// The command for connecting session
        /// </summary>
        public ICommand ConnectSessionCommand { get; set; }

        /// <summary>
        /// The command for disconnecting from session
        /// </summary>
        public ICommand DisconnectSessionCommand { get; set; }

        #endregion

        #endregion

        public MainViewModel()
        {
            NewSessionCommand = new RelayCommand(NewSession);
            SaveSessionCommand = new RelayCommand(SaveSession);
            OpenSessionCommand = new RelayCommand(OpenSession);
            ExitApplicationCommand = new RelayCommand(ExitApplication);
            ConnectSessionCommand = new RelayCommand(ConnectSession);
            DisconnectSessionCommand = new RelayCommand(DisconnectSession);

            AddVariable = new RelayCommand(Add);

            _uaClientApi = IoC.UaClientApi;

            NodetreeViewModel = new NodeTreeViewModel(SetSelectedNode);

        }

        private void Add()
        {
            var nodeId = new NodeId(_refDiscOfSelectedNode.NodeId.ToString());
            _nodeIdsToRead.Add(nodeId);

            //_refDiscOfSelectedNode.TypeId
            VariablesToRead.Add(new Variable()
            {
                NodeId = nodeId
            });

            _uaClientApi.AddMonitoredItem(nodeId, 1).Notification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);

            // ClientUtils.GetDataType(IoC.UaClientApi.Session, nodeId);
        }
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            var notification = e.NotificationValue as MonitoredItemNotification;
            if (notification == null)
            {
                return;
            }
            //monitoredItem.
            VariablesToRead.First(x => x.Name == monitoredItem.DisplayName).Value = notification.Value.Value;
        }

        private void NewSession()
        {
            IoC.Application.GoToPage(ApplicationPage.Welcome);
        }

        private void SaveSession()
        {
            return;
        }

        private void OpenSession()
        {
            return;
        }

        private void ExitApplication()
        {
            return;
        }
        private void ConnectSession()
        {
            return;
        }

        private void DisconnectSession()
        {
            _uaClientApi.Subscribe(2000);
            _uaClientApi.ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
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