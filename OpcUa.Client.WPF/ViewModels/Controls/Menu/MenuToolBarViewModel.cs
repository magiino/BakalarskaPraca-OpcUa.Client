using System.Windows.Input;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MenuToolBarViewModel : BaseViewModel
    { 
        #region Commands
        public ICommand DisconnectSessionCommand { get; set; }
        public ICommand SaveProjectCommand { get; set; }

        #endregion

        #region Constructor

        public MenuToolBarViewModel()
        {
            DisconnectSessionCommand = new MixRelayCommand(DisconnectSession);
            SaveProjectCommand = new MixRelayCommand(SaveProject, SaveprojectCanUse);
        }

        #endregion

        #region Command Method

        private void DisconnectSession(object parameter)
        {
            IoC.UaClientApi.Disconnect();
            IoC.Application.GoToPage(ApplicationPage.Welcome);
        }

        private void SaveProject(object parameter)
        {
            IoC.UnitOfWork.CompleteAsync();
        }

        #endregion

        #region CanUse methods

        private bool SaveprojectCanUse(object parameter)
        {
            return IoC.UnitOfWork.HasUnsavedChanges();
        } 

        #endregion
    }
}