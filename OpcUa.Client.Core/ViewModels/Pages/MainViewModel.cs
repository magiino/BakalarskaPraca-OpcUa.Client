namespace OpcUa.Client.Core
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Fields
        #endregion

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

        private int _selectedIndes = 0;
        public int SelectedIndex
        {
            get => _selectedIndes;
            set
            {
                //if (value == 2 & ChartViewModel == null)
                //    CreateChartVM();

                _selectedIndes = value;
            }

        }

        private void CreateChartVM()
        {
            //ChartViewModel = new ChartViewModel(IoC.DataContext);
        }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            var uaClientApi = IoC.UaClientApi;
            var unitOfWork = IoC.UnitOfWork;

            // Set up child view models
            NodetreeViewModel = new NodeTreeViewModel();
            NodeAttributesViewModel = new NodeAttributesViewModel(uaClientApi);
            NotificationViewModel = new NotificationViewModel(unitOfWork, uaClientApi);
            MenuBarViewModel = new MenuBarViewModel();
            MenuToolBarViewModel = new MenuToolBarViewModel();
            ArchiveViewModel = new ArchiveViewModel(unitOfWork, uaClientApi);
            ChartViewModel = new ChartViewModel(unitOfWork);
            ZoomChartViewModel = new ZoomChartViewModel(unitOfWork);
            LiveChartViewModel = new LiveChartViewModel(uaClientApi);
        }

        #endregion
    }
}