using System;

namespace OpcUa.Client.Core
{
    /// <summary>
    /// A base view model for any dialogs
    /// </summary>
    public class BaseDialogViewModel : BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// The title of the dialog
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Command for closing dialog window
        /// </summary>
        public Action CloseAction { get; set; } 
        #endregion
    }
}