using System.Diagnostics;
using System.Windows.Input;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class MenuBarViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly IUnitOfWork _iUnitOfWork;
        private ApplicationPage _goTo;
        #endregion

        #region Commands
        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand ExitApplicationCommand { get; }
        public ICommand OpenGitHubCommand { get; }
        #endregion

        #region Constructor
        public MenuBarViewModel(IUnitOfWork iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;

            NewProjectCommand = new MixRelayCommand((obj) => GoToPageIfSaved(ApplicationPage.Endpoints));
            SaveProjectCommand = new MixRelayCommand((obj) => _iUnitOfWork.CompleteAsync(), SaveprojectCanUse);
            OpenProjectCommand = new MixRelayCommand((obj) => GoToPageIfSaved(ApplicationPage.Welcome));
            ExitApplicationCommand = new MixRelayCommand((obj) =>
            {
                IoC.AppManager.Timer.Dispose();
                IoC.AppManager.CloseApplication();
            });
            OpenGitHubCommand = new MixRelayCommand((obj) => Process.Start("https://github.com/magiino/BakalarskaPraca-OpcUa.Client"));
        }
        #endregion

        #region Command Methods
        public void GoToPageIfSaved(ApplicationPage page)
        {
            if (!_iUnitOfWork.HasUnsavedChanges())
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
                _iUnitOfWork.CompleteAsync();

            IoC.AppManager.Timer.Dispose();
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

        private bool SaveprojectCanUse(object parameter)
        {
            return _iUnitOfWork.HasUnsavedChanges();
        }
        #endregion
    }
}