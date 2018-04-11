using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MainViewModel : BaseViewModel
    { 
        #region Public Properties

        public NodeTreeViewModel NodetreeViewModel { get; set; }
        public NodeAttributesViewModel NodeAttributesViewModel { get; set; }
        public NotificationViewModel NotificationViewModel { get; set; }
        public ArchiveViewModel ArchiveViewModel { get; set; }
        public MenuBarViewModel MenuBarViewModel { get; set; }
        public MenuToolBarViewModel MenuToolBarViewModel { get; set; }
        public ChartViewModel ChartViewModel { get; set; }
        public ZoomChartViewModel ZoomChartViewModel { get; set; }
        public  LiveChartViewModel LiveChartViewModel { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            // TODO prerobit cey view mdoel locator a nemusim mat tento VM vobec
            var uaClientApi = IoC.UaClientApi;
            var unitOfWork = IoC.UnitOfWork;
            var messenger = IoC.Messenger;

            // Set up child view models
            NodetreeViewModel = new NodeTreeViewModel();
            NodeAttributesViewModel = new NodeAttributesViewModel(uaClientApi, messenger);
            NotificationViewModel = new NotificationViewModel(unitOfWork, uaClientApi, messenger);
            MenuBarViewModel = new MenuBarViewModel();
            MenuToolBarViewModel = new MenuToolBarViewModel();
            ArchiveViewModel = new ArchiveViewModel(unitOfWork, uaClientApi, messenger);
            ChartViewModel = new ChartViewModel(unitOfWork);
            ZoomChartViewModel = new ZoomChartViewModel(unitOfWork, messenger);
            LiveChartViewModel = new LiveChartViewModel(uaClientApi, messenger);
        }

        #endregion
    }
}