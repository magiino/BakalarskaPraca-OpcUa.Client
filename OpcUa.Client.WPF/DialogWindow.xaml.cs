using System.Windows;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        #region Private Members

        /// <summary>
        /// The view model for this window
        /// </summary>
        private WindowDialogViewModel _viewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The view model for this window
        /// </summary>
        public WindowDialogViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                // Set new value
                _viewModel = value;

                // Update data context
                DataContext = _viewModel;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DialogWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}