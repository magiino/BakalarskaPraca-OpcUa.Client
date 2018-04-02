using System;
using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class LogInDialogViewModel : BaseDialogViewModel
    {
        public string UserName { get; set; }
        public ICommand LogInCommand { get; set; }

        public LogInDialogViewModel()
        {
            LogInCommand = new RelayParameterizedCommand(LogIn);
        }

        private void LogIn(object parameter)
        {
            MessengerInstance.Send(new SendCredentials(UserName, (parameter as IHavePassword)?.SecurePassword));

            CloseAction();
        }
    }
}
