using System.Windows.Input;

namespace OpcUA.Client.Core
{
    public class MenuToolBarViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;

        #endregion

        #region Public Properties

        #endregion

        #region Commands
        public ICommand ConnectSessionCommand { get; set; }
        public ICommand DisconnectSessionCommand { get; set; }

        #endregion

        #region Constructor

        public MenuToolBarViewModel(UaClientApi uaClientApi)
        {
            _uaClientApi = uaClientApi;

            ConnectSessionCommand = new RelayCommand(ConnectSession);
            DisconnectSessionCommand = new RelayCommand(DisconnectSession);
        }

        #endregion

        #region Command Methods

        private void ConnectSession() { }
        private void DisconnectSession() { }

        #endregion
    }
}