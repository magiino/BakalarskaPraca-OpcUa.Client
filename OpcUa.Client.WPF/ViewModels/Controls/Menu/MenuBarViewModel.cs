using System.Diagnostics;
using System.Windows.Input;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MenuBarViewModel : BaseViewModel
    {
        #region Private Fields
        private ApplicationPage _goTo;
        #endregion

        #region Commands

        public ICommand NewProjectCommand { get; set; }
        public ICommand SaveProjectCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }
        public ICommand OpenGitHubCommand { get; set; }

        #endregion

        #region Constructor

        public MenuBarViewModel()
        {
            NewProjectCommand = new RelayCommand((obj) => GoToPageIfSaved(ApplicationPage.Endpoints));
            SaveProjectCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(() => IoC.UnitOfWork.CompleteAsync(), SaveprojectCanUse);
            OpenProjectCommand = new RelayCommand((obj) => GoToPageIfSaved(ApplicationPage.Welcome));
            ExitApplicationCommand = new RelayCommand((obj) => IoC.AppManager.CloseApplication());
            OpenGitHubCommand = new RelayCommand((obj) => Process.Start("https://github.com/magiino/BakalarskaPraca-OpcUa.Client"));
        }

        #endregion

        #region Command Methods

        public void GoToPageIfSaved(ApplicationPage page)
        {
            if (!IoC.UnitOfWork.HasUnsavedChanges())
            {
                IoC.UaClientApi.Disconnect();
                IoC.Application.GoToPage(page);
                return;
            }

            _goTo = page;
            ShowOptionWindow();
        }

        private void Save(bool option)
        {
            if (option)
                IoC.UnitOfWork.CompleteAsync();

            IoC.Application.GoToPage(_goTo);
        }

        public void ShowOptionWindow()
        {
            IoC.Ui.ShowOption(new OptionDialogViewModel()
            {
                Title = "Save project",
                Message = "Do you want save project?",
                Option1 = "Yes",
                Option2 = "No",
                OptionAction = Save
            });
        }

        private bool SaveprojectCanUse()
        {
            return IoC.UnitOfWork.HasUnsavedChanges();
        }

        #endregion
    }
}