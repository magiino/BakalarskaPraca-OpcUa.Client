using System.Windows.Input;

namespace OpcUA.Client.Core
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;

        #endregion

        #region Public Properties

        public NodeTreeViewModel NodetreeViewModel { get; set; }
        public NodeAttributesViewModel NodeAttributesViewModel { get; set; }
        public SubscriptionViewModel SubscriptionViewModel { get; set; }
        public MenuBarViewModel MenuBarViewModel { get; set; }

        #endregion

        #region Commands

            #region ToolBar Commands

            public ICommand ConnectSessionCommand { get; set; }
            public ICommand DisconnectSessionCommand { get; set; }

            #endregion

        #endregion

        #region Constructor

        public MainViewModel()
        {
            _uaClientApi = IoC.UaClientApi;

        
            ConnectSessionCommand = new RelayCommand(ConnectSession);
            DisconnectSessionCommand = new RelayCommand(DisconnectSession);

            // Set up child view models
            NodetreeViewModel = new NodeTreeViewModel();
            NodeAttributesViewModel = new NodeAttributesViewModel(_uaClientApi);
            SubscriptionViewModel = new SubscriptionViewModel(_uaClientApi);
            MenuBarViewModel = new MenuBarViewModel(_uaClientApi);
        }

        #endregion

        #region Command Methods

      
        private void ConnectSession() {}
        private void DisconnectSession() {}

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