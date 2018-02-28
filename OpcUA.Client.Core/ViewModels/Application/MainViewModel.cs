using System;
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
        /// Selected Node in tree view
        /// </summary>
        public ReferenceDescription SelectedNode { get; set; }

        private ObservableCollection<ReferenceDescription> _selectedNodes = new ObservableCollection<ReferenceDescription>();

        public ObservableCollection<ReferenceDescription> SelectedNodes
        {
            get => _selectedNodes;
            set => _selectedNodes = value;
        }

        public void SetSelectedNode(ReferenceDescription selectedNode)
        {
            SelectedNode = selectedNode;
            _selectedNodes.Add(selectedNode);
        }

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
        // IsSelected h = SetSelectedNode;
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
                children.Select(content => new NodeItemViewModel(content, isSelectedDelegate)));
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
    }
}