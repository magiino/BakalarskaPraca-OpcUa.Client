using System.Windows.Input;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MenuToolBarViewModel : BaseViewModel
    { 
        #region Commands
        //public ICommand ConnectSessionCommand { get; set; }
        public ICommand DisconnectSessionCommand { get; set; }
        public ICommand SaveProjectCommand { get; set; }

        #endregion

        #region Constructor

        public MenuToolBarViewModel()
        {

            DisconnectSessionCommand = new RelayCommand(DisconnectSession);
            SaveProjectCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(SaveProject, SaveprojectCanUse);
        }

        #endregion

        #region Command Method

        private void DisconnectSession(object parameter)
        {
            IoC.UaClientApi.Disconnect();
            IoC.Application.GoToPage(ApplicationPage.Welcome);
        }

        private void SaveProject()
        {
            IoC.UnitOfWork.CompleteAsync();
        }

        #endregion

        private bool SaveprojectCanUse()
        {
            return IoC.UnitOfWork.HasUnsavedChanges();
        }
    }
}