using System;
using System.Threading;

namespace OpcUa.Client.Core
{
    public class AppManager
    {
        public Guid ProjectId { get; set; }
        public Action CloseAction { get; set; }

        public Timer Timer { get; set; }

        public void CloseApplication()
        {
            IoC.DisposeAll();
            CloseAction();
        }

        public void ShowExceptionErrorMessage(Exception e)
        {
            IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
            {
                Title = "Error",
                Message = e.Message,
                OkText = "Ok"
            });
        }

        public void ShowWarningMessage(string msg)
        {
            IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
            {
                Title = "Error",
                Message = msg,
                OkText = "Ok"
            });
        }
    }
}