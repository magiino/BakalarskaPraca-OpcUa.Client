using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MainViewModel : BaseViewModel
    {
        #region Public Properties
        public MenuBarViewModel MenuBarViewModel { get; }
        public MenuToolBarViewModel MenuToolBarViewModel { get; }

        public NodeTreeViewModel NodetreeViewModel { get; }
        public NodeAttributesViewModel NodeAttributesViewModel { get; }
        public NotificationViewModel NotificationViewModel { get; }
        public ArchiveViewModel ArchiveViewModel { get; }
        public ZoomChartViewModel ZoomChartViewModel { get; }
        public  LiveChartViewModel LiveChartViewModel { get; }
        #endregion

        #region Constructor
        public MainViewModel(IUnitOfWork iUnitOfWork, UaClientApi uaClientApi, Messenger messenger)
        {
            // Set up child view models
            MenuBarViewModel = new MenuBarViewModel(iUnitOfWork);
            MenuToolBarViewModel = new MenuToolBarViewModel(iUnitOfWork);
            NodetreeViewModel = new NodeTreeViewModel();
            NodeAttributesViewModel = new NodeAttributesViewModel(uaClientApi, messenger);
            NotificationViewModel = new NotificationViewModel(iUnitOfWork, uaClientApi, messenger);
            ArchiveViewModel = new ArchiveViewModel(iUnitOfWork, uaClientApi, messenger);
            ZoomChartViewModel = new ZoomChartViewModel(iUnitOfWork, messenger);
            LiveChartViewModel = new LiveChartViewModel(uaClientApi, messenger);
        }
        #endregion
    }
}