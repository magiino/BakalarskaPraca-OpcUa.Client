using System;
using System.Windows.Input;

namespace OpcUa.Client.Core
{ 
    /// <summary>
    /// Details for a message box dialog
    /// </summary>
    public class OptionDialogViewModel : BaseDialogViewModel
    {
        public string Message { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }

        public ICommand Option1Command { get; set; }
        public ICommand Option2Command { get; set; }

        public Action<bool> OptionAction { get; set; }

        public OptionDialogViewModel()
        {
            Option1Command = new RelayCommand(Option1Cmd);
            Option2Command = new RelayCommand(Option2Cmd);
        }

        private void Option1Cmd()
        {
            OptionAction(true);
            CloseAction();
        }

        private void Option2Cmd()
        {
            OptionAction(false);
            CloseAction();
        }
    }
}
