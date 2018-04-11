using System.Windows.Controls;
using PropertyChanged;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// The View Model for the custom flat window
    /// </summary>
    [DoNotNotify]
    public class DialogWindowViewModel : WindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The title of this dialog window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content to host inside the dialog
        /// </summary>
        public Control Content { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DialogWindowViewModel()
        {
            // Make minimum size smaller
            WindowMinimumWidth = 250;
            WindowMinimumHeight = 100;
        }

        #endregion
    }
}
