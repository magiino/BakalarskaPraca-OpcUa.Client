using System;

namespace OpcUa.Client.Core
{
    public class AppManager
    {
        public Guid ProjectId { get; set; }
        public Action CloseAction { get; set; }

        public void CloseApplication()
        {
            IoC.DisposeAll();
            CloseAction();
        }

        public void ShowErrorMessage(Exception e)
        {
            IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
            {
                Title = "Error",
                Message = e.Message,
                OkText = "Ok"
            });
        }
    }
}