using System.Security;
using OpcUA.Client.Core;

namespace OpcUA.Client
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
