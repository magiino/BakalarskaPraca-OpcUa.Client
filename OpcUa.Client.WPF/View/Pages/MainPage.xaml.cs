namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : BasePage<MainViewModel>
    {
        public MainPage(MainViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}