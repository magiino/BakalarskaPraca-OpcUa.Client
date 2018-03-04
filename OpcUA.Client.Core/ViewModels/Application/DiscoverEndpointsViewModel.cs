using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using OpcUA.Client.Core.DataModels;

namespace OpcUA.Client.Core
{
    public class DiscoverEndpointsViewModel : BaseViewModel
    {
        #region Private Fields

        /// <summary>
        /// Ua client
        /// </summary>
        private readonly UAClientHelperAPI _uaClient; 

        #endregion

        #region Public Properties

        /// <summary>
        /// Url of server for discover endpoints
        /// </summary>
        public string DiscoveryUrl { get; set; }

        /// <summary>
        /// A list of discovered endpoints
        /// </summary>
        public ObservableCollection<EndpointDataGridModel> DiscoveredEndpoints { get; set; } = new ObservableCollection<EndpointDataGridModel>();

        /// <summary>
        /// Url of server for discover endpoints
        /// </summary>
        public string SessionName { get; set; }

        /// <summary>
        /// Protocols in combo box
        /// </summary>
        public IEnumerable<EProtocol> EProtocols { get; set; } = Enum.GetValues(typeof(EProtocol)).Cast<EProtocol>();

        /// <summary>
        /// Selected protocol form combo box
        /// </summary>
        public EProtocol SelectedProtocol { get; set; }

        public bool? NoneIsSelected { get; set; } = true;

        public bool? SignIsSelected { get; set; } = false;

        public bool? SignEncryptIsSelected { get; set; } = false;

        public bool? Basic128Rsa15IsSelected { get; set; } = false;

        public bool? Basic256IsSelected { get; set; } = false;

        public bool? Basic256Sha256IsSelected { get; set; } = false;
        
        /// <summary>
        /// Protocols in combo box
        /// </summary>
        public IEnumerable<EMessageEncoding> EMessageEncodings { get; set; } = Enum.GetValues(typeof(EMessageEncoding)).Cast<EMessageEncoding>();

        /// <summary>
        /// Selected protocol form combo box
        /// </summary>
        public EMessageEncoding SelectedEncoding { get; set; }

        public bool? AnonymousIsSelected { get; set; } = true;
        
        public bool? UserPwIsSelected { get; set; } = false;

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command for search endpoints
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// The command for search all endpoints in network
        /// </summary>
        public ICommand ConnectCommand { get; set; }

        #endregion

        #region Constructor

        public DiscoverEndpointsViewModel()
        {
            _uaClient = IoC.Get<UAClientHelperAPI>();

            SearchCommand = new RelayCommand(SearchEndpoints);
            ConnectCommand = new RelayParameterizedCommand(ConnectToServer);
        }

        private void ConnectToServer(object parameter)
        {
            var userName = UserName;
            var pass = (parameter as IHavePassword)?.SecurePassword.Unsecure();
        }

        #endregion

        private void SearchEndpoints()
        {
            var servers = _uaClient.FindServers(DiscoveryUrl);
          
            foreach (ApplicationDescription ad in servers)
            {
                foreach (string url in ad.DiscoveryUrls)
                {
                    var endpoints = _uaClient.GetEndpoints(DiscoveryUrl);
                    foreach (EndpointDescription ep in endpoints)
                    {
                        DiscoveredEndpoints.Add(new EndpointDataGridModel(ep));
                    }
                }
            }
        }
    }
}
