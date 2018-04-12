using System.Windows.Input;

namespace OpcUa.Client.Core
{ 
    /// <summary>
    /// Details for a message box dialog
    /// </summary>
    public class MessageBoxDialogViewModel : BaseDialogViewModel
    {
        #region Public Properties
        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The text to use for the OK button
        /// </summary>
        public string OkText { get; set; }
        #endregion

        #region Commands
        public ICommand CloseActionCommand { get; }
        #endregion

        #region Constructor
        public MessageBoxDialogViewModel()
        {
            CloseActionCommand = new RelayCommand((obj) => CloseAction());
        } 
        #endregion
    }
}