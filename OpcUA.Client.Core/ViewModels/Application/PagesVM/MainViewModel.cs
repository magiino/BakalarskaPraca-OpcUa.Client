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
        public ArchiveViewModel ArchiveViewModel { get; set; }
        public MenuBarViewModel MenuBarViewModel { get; set; }
        public MenuToolBarViewModel MenuToolBarViewModel { get; set; }
        public ChartViewModel ChartViewModel { get; set; }

        private int _selectedIndes = 0;
        public int SelectedIndex
        {
            get => _selectedIndes;
            set
            {
                if (value == 2 & ChartViewModel == null)
                    CreateChartVM();

                _selectedIndes = value;
            }

        }

        private void CreateChartVM()
        {
            ChartViewModel = new ChartViewModel(IoC.DataContext);
        }

        #endregion

        #region Commands
        #endregion

        #region Constructor

        public MainViewModel()
        {
            var uaClientApi = IoC.UaClientApi;
            var dataContext = IoC.DataContext;

            // Set up child view models
            NodetreeViewModel = new NodeTreeViewModel();
            NodeAttributesViewModel = new NodeAttributesViewModel(uaClientApi);
            SubscriptionViewModel = new SubscriptionViewModel(uaClientApi);
            MenuBarViewModel = new MenuBarViewModel(uaClientApi);
            MenuToolBarViewModel = new MenuToolBarViewModel(uaClientApi);
            ArchiveViewModel = new ArchiveViewModel(dataContext, uaClientApi);
            ChartViewModel = new ChartViewModel(IoC.DataContext);
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