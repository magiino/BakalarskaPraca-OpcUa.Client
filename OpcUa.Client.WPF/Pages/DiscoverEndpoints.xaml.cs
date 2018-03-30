using System.Security;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for DiscoverEndpoints.xaml
    /// </summary>
    public partial class DiscoverEndpoints : BasePage<DiscoverEndpointsViewModel>, IHavePassword
    {
        public DiscoverEndpoints()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The secure password for this login page
        /// </summary>
        public SecureString SecurePassword => PasswordText.SecurePassword;
    }
}
