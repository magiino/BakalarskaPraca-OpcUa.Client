namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : BasePage<WelcomeViewModel>
    {
        public WelcomePage(WelcomeViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}