using System;
using System.Windows.Input;

namespace OpcUa.Client.Core
{ 
    /// <summary>
    /// Details for a message box dialog
    /// </summary>
    public class OptionDialogViewModel : BaseDialogViewModel
    {
        #region Public Properties
        public string Message { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }

        public Action<bool> OptionAction { get; set; }
        #endregion

        #region Commands
        public ICommand Option1Command { get; }
        public ICommand Option2Command { get; }
        #endregion

        #region Constructor
        public OptionDialogViewModel()
        {
            Option1Command = new RelayCommand(Option1Cmd);
            Option2Command = new RelayCommand(Option2Cmd);
        }
        #endregion

        #region Command Methods
        private void Option1Cmd(object parameter)
        {
            OptionAction(true);
            CloseAction();
        }

        private void Option2Cmd(object parameter)
        {
            OptionAction(false);
            CloseAction();
        } 
        #endregion
    }
}