using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class LogInDialogViewModel : BaseDialogViewModel
    {
        #region Private Fields
        private readonly Messenger _messenger; 
        #endregion

        #region Public Properties
        public string UserName { get; set; } 
        #endregion

        #region Commands
        public ICommand LogInCommand { get; }
        #endregion

        #region Constructor
        public LogInDialogViewModel(Messenger messenger)
        {
            _messenger = messenger;
            LogInCommand = new RelayCommand(LogIn);
        }
        #endregion

        #region Command Methods
        private void LogIn(object parameter)
        {
            _messenger.Send(new SendCredentials(UserName, (parameter as IHavePassword)?.SecurePassword));

            CloseAction();
        } 
        #endregion
    }
}