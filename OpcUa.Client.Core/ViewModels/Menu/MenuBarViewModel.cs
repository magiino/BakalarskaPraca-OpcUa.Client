using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class MenuBarViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;

        #endregion

        #region Public Properties

        #endregion

        #region Commands

        public ICommand NewSessionCommand { get; set; }
        public ICommand SaveSessionCommand { get; set; }
        public ICommand OpenSessionCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }

        #endregion

        #region Constructor

        public MenuBarViewModel(UaClientApi uaClientApi)
        {
            _uaClientApi = uaClientApi;

            NewSessionCommand = new RelayCommand(NewSession);
            SaveSessionCommand = new RelayCommand(SaveSession);
            OpenSessionCommand = new RelayCommand(OpenSession);
            ExitApplicationCommand = new RelayCommand(ExitApplication);
        }

        #endregion

        #region Command Methods

        private void NewSession()
        {
        }

        private void SaveSession()
        {
            _uaClientApi.SaveConfiguration();
        }

        private void OpenSession()
        {
        }

        private void ExitApplication()
        {
        }

        #endregion
    }
}