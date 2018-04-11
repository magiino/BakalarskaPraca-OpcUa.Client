using System;
using OpcUa.Client.Core;

namespace OpcUa.Client.Core
{
    /// <summary>
    /// A base view model for any dialogs
    /// </summary>
    public class BaseDialogViewModel : BaseViewModel
    {
        /// <summary>
        /// The title of the dialog
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Command for closing dialog window
        /// </summary>
        public Action CloseAction { get; set; }
    }
}
