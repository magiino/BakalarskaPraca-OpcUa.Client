using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class DiscoverEndpointsViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly UaClientApi _uaClientApi;

        private ApplicationDescription _selectedServer;
        private EndpointDescription _selectedEndpoint;
        private readonly EndpointDescriptionCollection _discoveredEndpoints = new EndpointDescriptionCollection();
        #endregion

        #region Public Properties
        public string DiscoveryUrl { get; set; } = "opc.tcp://A05-226b:48010";

        public ObservableCollection<ApplicationDescription> FoundedServers { get; set; }
        public ApplicationDescription SelectedServer
        {
            get =>_selectedServer;
            set
            {
                _selectedServer = value;
                EndpointFilter(null);
            }
        }

        public ObservableCollection<EndpointListModel> FilteredEndpoints { get; set; }
        public EndpointListModel SelectedEndpointListModel
        {
            set
            {
                _selectedEndpoint = value?.EndpointDesciption;
                SetMessegeEncoding();
            }
        }

            #region Filter
            public string SessionName { get; set; } = "MySession";
            public string ProjectName { get; set; } = "MyProject";

            public IEnumerable<TransportProtocol> EProtocols { get; set; } = Enum.GetValues(typeof(TransportProtocol)).Cast<TransportProtocol>();
            private TransportProtocol _selectedTransportProtocol;
            public TransportProtocol SelectedTransportProtocol
            {
                get => _selectedTransportProtocol;
                set
                {
                    _selectedTransportProtocol = value;
                    EndpointFilter(null);
                }
            }

            public bool NoneIsSelected { get; set; } = true;
            public bool SignIsSelected { get; set; } = true;
            public bool SignEncryptIsSelected { get; set; } = true;

            public bool Basic128Rsa15IsSelected { get; set; } = true;
            public bool Basic256IsSelected { get; set; } = true;
            public bool Basic256Sha256IsSelected { get; set; } = true;

            public IList<MessageEncoding> EMessageEncodings { get; set; } = Enum.GetValues(typeof(MessageEncoding)).Cast<MessageEncoding>().ToList();
            public MessageEncoding SelectedEncoding { get; set; }

            public bool AnonymousIsSelected { get; set; } = true;
            public bool UserPwIsSelected { get; set; } = false;
            public string UserName { get; set; } 
            #endregion
        #endregion

        #region Commands
        public ICommand SearchCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand StartFilterCommand { get; }
        public ICommand LoadProjectCommand { get; }
        #endregion

        #region Constructor
        public DiscoverEndpointsViewModel(IUnitOfWork iUnitOfWork, UaClientApi uaClientApi)
        {
            _iUnitOfWork = iUnitOfWork;
            _uaClientApi = uaClientApi;
            
            SearchCommand = new MixRelayCommand(SearchEndpoints);
            ConnectCommand = new MixRelayCommand(ConnectToServer);
            StartFilterCommand = new MixRelayCommand(EndpointFilter);
            LoadProjectCommand = new MixRelayCommand(LoadprojectPage);
        }

        #endregion

        #region Command Methods
        private void ConnectToServer(object parameter)
        {
            try
            {
                if (UserPwIsSelected)
                {
                    // TODO ak uz je taky ucet v databaze neregistrovat?
                    _uaClientApi.Connect(_selectedEndpoint, UserName, (parameter as IHavePassword)?.SecurePassword.Unsecure(), SessionName);
                    IoC.AppManager.ProjectId = _iUnitOfWork.Projects.Add(new ProjectEntity()
                    {
                        Name = ProjectName,
                        SessionName = SessionName,
                        Endpoint = Mapper.CreateEndpointEntity(_selectedEndpoint),
                        User = _iUnitOfWork.Auth.Register(new UserEntity(){UserName = UserName}, (parameter as IHavePassword)?.SecurePassword)
                    }).Id;
                }
                else
                {
                    _uaClientApi.ConnectAnonymous(_selectedEndpoint, SessionName);
                    IoC.AppManager.ProjectId = _iUnitOfWork.Projects.Add(new ProjectEntity()
                    {
                        Name = ProjectName,
                        SessionName = SessionName,
                        Endpoint = Mapper.CreateEndpointEntity(_selectedEndpoint),
                    }).Id;
                }
            }
            catch (Exception e)
            {
                IoC.AppManager.ShowErrorMessage(e);
            }

            IoC.Application.GoToPage(ApplicationPage.Main);
        }

        private void LoadprojectPage(object parameter)
        {
            IoC.Application.GoToPage(ApplicationPage.Welcome);
        }

        private void SearchEndpoints(object parameter)
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

            EndpointFilter(null);
        }
        #endregion

        #region Endpoints Filter Methods
        private void EndpointFilter(object parameter)
        {
            var filterProtocol = EndpointUtils.SelectByProtocol(_discoveredEndpoints, SelectedTransportProtocol);
            var filterSecurityMode = EndpointUtils.SelectByMessageSecurityModes(filterProtocol, GetSelectedModes());
            var filterSecurityPolciies = EndpointUtils.SelectBySecurityPolicies(filterSecurityMode, GetSelectedPolicies());
            var filterServer = EndpointUtils.SelectByApplicationName(filterSecurityPolciies, SelectedServer?.ApplicationName.ToString());

            FilteredEndpoints = new ObservableCollection<EndpointListModel>(filterServer.Select(x => new EndpointListModel(x)));
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
            var securityPolicies = Enum.GetValues(typeof(SecurityPolicy)).Cast<SecurityPolicy>().ToList();

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
            if (_selectedEndpoint == null) return;

            var num = _selectedEndpoint.TransportProfileUri.Contains("xml") ? 1 : 0;
            num = (_selectedEndpoint.TransportProfileUri.Contains("xml") && _selectedEndpoint.TransportProfileUri.Contains("binary")) ? 2 : num;

            switch (num)
            {
                case 0:
                    EMessageEncodings = new List<MessageEncoding>() { MessageEncoding.Binary };
                    SelectedEncoding = EMessageEncodings.First();
                    break;
                case 1:
                    EMessageEncodings = new List<MessageEncoding>() { MessageEncoding.Xml };
                    SelectedEncoding = EMessageEncodings.First();
                    break;
                case 2:
                    EMessageEncodings = new List<MessageEncoding>() { MessageEncoding.Binary, MessageEncoding.Xml };
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