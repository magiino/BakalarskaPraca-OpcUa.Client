using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class DiscoverEndpointsViewModel : BaseViewModel
    {
        #region Private Fields

        /// <summary>
        /// Ua Client API
        /// </summary>
        private readonly UaClientApi _uaClientApi; 

        #endregion

        #region Public Properties

        /// <summary>
        /// Url of server for discover endpoints
        /// </summary>
        public string DiscoveryUrl { get; set; } = "opc.tcp://localhost";

        /// <summary>
        /// Founded servers from search
        /// </summary>
        public ObservableCollection<ApplicationDescription> FoundedServers { get; set; }

        /// <summary>
        /// Selected server from <see cref="FoundedServers"/>
        /// </summary>
        public ApplicationDescription SelectedServer { get; set; }

        /// <summary>
        /// A list of discovered endpoints
        /// </summary>
        public ObservableCollection<EndpointDataGridModel> DiscoveredEndpoints { get; set; } = new ObservableCollection<EndpointDataGridModel>();

        /// <summary>
        /// Getting selected endpoint description from data grid model
        /// </summary>
        public EndpointDataGridModel SelectedEndpointDataGridModel
        {
            set => SelectedEndpoint = value?.EndpointDesciption;
        }

        /// <summary>
        /// Selected endpoint description
        /// </summary>
        public EndpointDescription SelectedEndpoint { get; set; }

            #region Filter
            /// <summary>
            /// Url of server for discover endpoints
            /// </summary>
            public string SessionName { get; set; }

            /// <summary>
            /// Protocols in combo box
            /// </summary>
            public IEnumerable<EProtocol> EProtocols { get; set; } = Enum.GetValues(typeof(EProtocol)).Cast<EProtocol>();

            /// <summary>
            /// Selected protocol from combo box
            /// </summary>
            public EProtocol SelectedProtocol { get; set; }

            public bool NoneIsSelected { get; set; } = true;

            public bool SignIsSelected { get; set; } = true;

            public bool SignEncryptIsSelected { get; set; } = true;

            public bool Basic128Rsa15IsSelected { get; set; } = true;

            public bool Basic256IsSelected { get; set; } = true;

            public bool Basic256Sha256IsSelected { get; set; } = true;

            /// <summary>
            /// Protocols in combo box
            /// </summary>
            public IEnumerable<EMessageEncoding> EMessageEncodings { get; set; } = Enum.GetValues(typeof(EMessageEncoding)).Cast<EMessageEncoding>();

            /// <summary>
            /// Selected protocol form combo box
            /// </summary>
            public EMessageEncoding SelectedEncoding { get; set; }

            public bool AnonymousIsSelected { get; set; } = true;

            public bool UserPwIsSelected { get; set; } = false;

            public string UserName { get; set; } 

            #endregion

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
            _uaClientApi = IoC.UaClientApi;
            
            SearchCommand = new RelayCommand(SearchEndpoints);
            ConnectCommand = new RelayParameterizedCommand(ConnectToServer);
        }

        #endregion

        #region Command Methods

        private void ConnectToServer(object parameter)
        {
            if (UserPwIsSelected)
            {
                var userName = UserName;
                var pass = (parameter as IHavePassword)?.SecurePassword.Unsecure();
            }
            else
            {
                _uaClientApi.SaveConfiguration();
                _uaClientApi.Connect(SelectedEndpoint, UserPwIsSelected, null, null);
                IoC.Application.GoToPage(ApplicationPage.Main);
            }
        }

        private void SearchEndpoints()
        {
            DiscoveredEndpoints.Clear();

            FoundedServers = new ObservableCollection<ApplicationDescription>(_uaClientApi.FindServers(DiscoveryUrl)); 

            foreach (ApplicationDescription ad in FoundedServers)
            {
                foreach (string url in ad.DiscoveryUrls)
                {
                    var endpoints = _uaClientApi.GetEndpoints(url);
                    foreach (EndpointDescription ep in endpoints)
                    {
                        var b = ep.EncodingSupport;
                        var c = ep.SecurityLevel;
                        var d = ep.UserIdentityTokens;
                        DiscoveredEndpoints.Add(new EndpointDataGridModel(ep));
                    }
                }
            }
        }

        #endregion
    }
}