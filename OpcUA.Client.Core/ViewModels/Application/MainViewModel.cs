using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public delegate void IsSelected(ReferenceDescription selectedNode);

    public class MainViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A list of root nodes in the address space
        /// </summary>
        public ObservableCollection<NodeItemViewModel> Items { get; set; }

        /// <summary>
        /// A list of attributes and values of <see cref="ReferenceDescription"/> object
        /// </summary>
        public ObservableCollection<AttributeDataGridModel> SelectedNode { get; set; } = new ObservableCollection<AttributeDataGridModel>();

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

            // Get the root nodes
            var children = IoC.Get<UAClientHelperAPI>().BrowseRoot();

            IsSelected isSelectedDelegate = SetSelectedNode;

            // Create the view models from the root ndoes
            Items = new ObservableCollection<NodeItemViewModel>(
                children.Select(content => new NodeItemViewModel(content, isSelectedDelegate)).OrderBy(x => x.Name));
        }

        private void NewSession()
        {
            IoC.Get<ApplicationViewModel>().GoToPage(ApplicationPage.Welcome);
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
            return;
        }

        /// <summary>
        /// Callback method for set up selected node
        /// </summary>
        /// <param name="selectedNode"></param>
        public void SetSelectedNode(ReferenceDescription selectedNode)
        {
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

            var node = IoC.Get<UAClientHelperAPI>().ReadNode(referenceDescription.NodeId.ToString());
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