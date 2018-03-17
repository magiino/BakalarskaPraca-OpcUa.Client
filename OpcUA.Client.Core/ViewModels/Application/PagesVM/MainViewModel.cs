using System.Windows.Input;

namespace OpcUA.Client.Core
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Fields
        #endregion

        #region Public Properties

        public NodeTreeViewModel NodetreeViewModel { get; set; }
        public NodeAttributesViewModel NodeAttributesViewModel { get; set; }
        public SubscriptionViewModel SubscriptionViewModel { get; set; }
        public MenuBarViewModel MenuBarViewModel { get; set; }
        public MenuToolBarViewModel MenuToolBarViewModel { get; set; }

        #endregion

        #region Commands
        #endregion

        #region Constructor

        public MainViewModel()
        {
            var uaClientApi = IoC.UaClientApi;

            // Set up child view models
            NodetreeViewModel = new NodeTreeViewModel();
            NodeAttributesViewModel = new NodeAttributesViewModel(uaClientApi);
            SubscriptionViewModel = new SubscriptionViewModel(uaClientApi);
            MenuBarViewModel = new MenuBarViewModel(uaClientApi);
            MenuToolBarViewModel = new MenuToolBarViewModel(uaClientApi);
        }

        #endregion

        #region Command Methods
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