using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class LogInDialogViewModel : BaseDialogViewModel
    {
        private readonly Messenger _messenger;

        public string UserName { get; set; }
        public ICommand LogInCommand { get; set; }

        public LogInDialogViewModel(Messenger messenger)
        {
            _messenger = messenger;
            LogInCommand = new RelayCommand(LogIn);
        }

        private void LogIn(object parameter)
        {
            _messenger.Send(new SendCredentials(UserName, (parameter as IHavePassword)?.SecurePassword));

            CloseAction();
        }
    }
}
