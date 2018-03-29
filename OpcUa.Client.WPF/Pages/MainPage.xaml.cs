using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : BasePage<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
        }
    }
}
