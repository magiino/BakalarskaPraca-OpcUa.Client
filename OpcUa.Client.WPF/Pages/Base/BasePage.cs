using System.Windows.Controls;
using OpcUA.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BasePage : Page
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePage()
        {
        }
    }

    /// <summary>
    /// A base page with added ViewModel support
    /// </summary>
    public class BasePage<VM> : BasePage
        where VM : BaseViewModel, new()
    {
        #region Private Member

        /// <summary>
        /// The view model associated with this page
        /// </summary>
        private VM _viewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        public VM ViewModel
        {
            get => _viewModel;
            set
            {
                // If nothing has changed, return
                if (_viewModel == value)
                    return;

                // Update the value
                _viewModel = value;

                // Set the data context for this page
                DataContext = _viewModel;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePage()
        {
            // Create a default view model
            ViewModel = new VM();
        }

        #endregion
    }
}
