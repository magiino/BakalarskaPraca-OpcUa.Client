using System;
using System.Threading;
using System.Windows.Input;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MenuToolBarViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly UaClientApi _uaClientApi;
        private Timer _timer;
        #endregion

        public bool SessionIsActive { get; set; }

        #region Commands
        public ICommand DisconnectSessionCommand { get; }
        public ICommand SaveProjectCommand { get; }
        #endregion

        #region Constructor
        public MenuToolBarViewModel(IUnitOfWork iUnitOfWork, UaClientApi uaClientApi)
        {
            _iUnitOfWork = iUnitOfWork;
            _uaClientApi = uaClientApi;

            DisconnectSessionCommand = new MixRelayCommand(DisconnectSession);
            SaveProjectCommand = new MixRelayCommand(SaveProject, SaveprojectCanUse);

            _timer = new Timer(SessionState, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
        }
        #endregion

        #region Command Methods
        private void DisconnectSession(object parameter)
        {
            _uaClientApi.Disconnect();
            IoC.Application.GoToPage(ApplicationPage.Welcome);
        }

        private void SaveProject(object parameter)
        {
            _iUnitOfWork.CompleteAsync();
        }
        #endregion

        #region CanUse methods
        private bool SaveprojectCanUse(object parameter)
        {
            return _iUnitOfWork.HasUnsavedChanges();
        }

        private void SessionState(object parameter)
        {
            SessionIsActive = _uaClientApi.SessionIsActive();
        }
        #endregion
    }
}