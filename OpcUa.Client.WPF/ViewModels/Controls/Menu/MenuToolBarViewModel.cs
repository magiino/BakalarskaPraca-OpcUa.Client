using System.Windows.Input;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MenuToolBarViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly IUnitOfWork _iUnitOfWork; 
        #endregion

        #region Commands
        public ICommand DisconnectSessionCommand { get; }
        public ICommand SaveProjectCommand { get; }
        #endregion

        #region Constructor
        public MenuToolBarViewModel(IUnitOfWork iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;

            DisconnectSessionCommand = new MixRelayCommand(DisconnectSession);
            SaveProjectCommand = new MixRelayCommand(SaveProject, SaveprojectCanUse);
        }
        #endregion

        #region Command Methods
        private void DisconnectSession(object parameter)
        {
            IoC.UaClientApi.Disconnect();
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
        #endregion
    }
}