using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Ninject.Infrastructure.Language;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class DiscoverEndpointsViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi; 

        #endregion

        #region Public Properties

        public string DiscoveryUrl { get; set; } = "opc.tcp://A05-226b:48010";

        public ObservableCollection<ApplicationDescription> FoundedServers { get; set; }

        private ApplicationDescription _selectedServer;

        public ApplicationDescription SelectedServer
        {
            get =>_selectedServer;
            set
            {
                _selectedServer = value;
                EndpointFilter();
            }
        }

        private readonly EndpointDescriptionCollection _discoveredEndpoints = new EndpointDescriptionCollection();

        public ObservableCollection<EndpointGridViewModel> FilteredEndpoints { get; set; }

        public EndpointGridViewModel SelectedEndpointGridViewModel
        {
            set
            {
                SelectedEndpoint = value?.EndpointDesciption;
                SetMessegeEncoding();
            }
        }

        public EndpointDescription SelectedEndpoint { get; set; }

            #region Filter

            public string SessionName { get; set; }

            public IEnumerable<EProtocol> EProtocols { get; set; } = Enum.GetValues(typeof(EProtocol)).Cast<EProtocol>();


            private EProtocol _selectedProtocol;

            public EProtocol SelectedProtocol
            {
                get => _selectedProtocol;
                set
                {
                    _selectedProtocol = value;
                    EndpointFilter();
                }
            }

            public bool NoneIsSelected { get; set; } = true;

            public bool SignIsSelected { get; set; } = true;

            public bool SignEncryptIsSelected { get; set; } = true;

            public bool Basic128Rsa15IsSelected { get; set; } = true;

            public bool Basic256IsSelected { get; set; } = true;

            public bool Basic256Sha256IsSelected { get; set; } = true;

            public IList<EMessageEncoding> EMessageEncodings { get; set; } = Enum.GetValues(typeof(EMessageEncoding)).Cast<EMessageEncoding>().ToList();

            public EMessageEncoding SelectedEncoding { get; set; }

            public bool AnonymousIsSelected { get; set; } = true;

            public bool UserPwIsSelected { get; set; } = false;

            public string UserName { get; set; } 

            #endregion

        #endregion

        #region Commands

        public ICommand SearchCommand { get; set; }

        public ICommand ConnectCommand { get; set; }

        public ICommand StartFilterCommand { get; set; }

        #endregion

        #region Constructor

        public DiscoverEndpointsViewModel()
        {
            _uaClientApi = IoC.UaClientApi;
            
            SearchCommand = new RelayCommand(SearchEndpoints);
            ConnectCommand = new RelayParameterizedCommand(ConnectToServer);
            StartFilterCommand = new RelayCommand(EndpointFilter);
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
                //_uaClientApi.SaveConfiguration();
                try
                {
                    _uaClientApi.Connect(SelectedEndpoint, UserPwIsSelected, null, null);
                    IoC.Application.GoToPage(ApplicationPage.Main);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message, "Error");
                }

            }
        }

        private void SearchEndpoints()
        {
            _discoveredEndpoints.Clear();

            try
            {
                FoundedServers = new ObservableCollection<ApplicationDescription>(_uaClientApi.FindServers(DiscoveryUrl));

                foreach (var server in FoundedServers)
                {
                    foreach (var url in server.DiscoveryUrls)
                    {
                        var endpoints = _uaClientApi.GetEndpoints(url);
                        foreach (var endpoint in endpoints)
                            _discoveredEndpoints.Add(endpoint);
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Error");
            }

            SelectedServer = FoundedServers?.First();

            EndpointFilter();
        }

        private void EndpointFilter()
        {
            var filterProtocol = EndpointUtils.SelectByProtocol(_discoveredEndpoints, SelectedProtocol);
            var filterSecurityMode = EndpointUtils.SelectByMessageSecurityModes(filterProtocol, GetSelectedModes());
            var filterSecurityPolciies = EndpointUtils.SelectBySecurityPolicies(filterSecurityMode, GetSelectedPolicies());
            var filterServer = EndpointUtils.SelectByApplicationName(filterSecurityPolciies, SelectedServer?.ApplicationName.ToString());

            FilteredEndpoints = new ObservableCollection<EndpointGridViewModel>(filterServer.Select( x => new EndpointGridViewModel(x)) );
        }

        private List<MessageSecurityMode> GetSelectedModes()
        {
            var securityModes = Enum.GetValues(typeof(MessageSecurityMode)).Cast<MessageSecurityMode>().ToList();

            List<MessageSecurityMode> selectedModes = new List<MessageSecurityMode>();

            if (NoneIsSelected)
                selectedModes.Add(securityModes[1]);
            if (SignIsSelected)
                selectedModes.Add(securityModes[2]);
            if (SignEncryptIsSelected)
                selectedModes.Add(securityModes[3]);

            return selectedModes;
        }

        private List<string> GetSelectedPolicies()
        {
            var securityPolicies = Enum.GetValues(typeof(ESecurityPolicy)).Cast<ESecurityPolicy>().ToList();

            List<string> selectedPolicies = new List<string>();

            if (NoneIsSelected)
                selectedPolicies.Add(EndpointUtils.ESecutityPolicyToString(securityPolicies[0]));
            if (Basic128Rsa15IsSelected)
                selectedPolicies.Add(EndpointUtils.ESecutityPolicyToString(securityPolicies[1]));
            if (Basic256IsSelected)
                selectedPolicies.Add(EndpointUtils.ESecutityPolicyToString(securityPolicies[2]));
            if (Basic256Sha256IsSelected)
                selectedPolicies.Add(EndpointUtils.ESecutityPolicyToString(securityPolicies[4]));

            return selectedPolicies;
        }

        private void SetMessegeEncoding()
        {
            if(SelectedEndpoint == null) return;
            
            var num = SelectedEndpoint.TransportProfileUri.Contains("xml") ? 1 : 0;
            num = (SelectedEndpoint.TransportProfileUri.Contains("xml") && SelectedEndpoint.TransportProfileUri.Contains("binary")) ? 2 : num;

            switch (num)
            {
                case 0:
                    EMessageEncodings = new List<EMessageEncoding>(){ EMessageEncoding.Binary};
                    SelectedEncoding = EMessageEncodings.First();
                    break;
                case 1:
                    EMessageEncodings = new List<EMessageEncoding>() { EMessageEncoding.Xml };
                    SelectedEncoding = EMessageEncodings.First();
                    break;
                case 2:
                    EMessageEncodings = new List<EMessageEncoding>() { EMessageEncoding.Binary, EMessageEncoding.Xml };
                    SelectedEncoding = EMessageEncodings.First();
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }


        #endregion
    }
}