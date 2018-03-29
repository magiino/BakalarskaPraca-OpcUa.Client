using System.Windows;
using System.Windows.Controls;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for PageHost.xaml
    /// </summary>
    public partial class PageHost : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// Register <see cref="CurrentPage"/> as a dependency property
        /// </summary>
        private static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(BasePage), typeof(PageHost), new UIPropertyMetadata(CurrentPagePropertyChanged));

        /// <summary>
        /// The current page to show in the page host
        /// </summary>
        public BasePage CurrentPage
        {
            get => (BasePage)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        } 

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageHost()
        {
            InitializeComponent();
        }

        #endregion


        #region Property Changed Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CurrentPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Get the frames
            var newPageFrame = (d as PageHost)?.NewPage;
            var oldPageFrame = (d as PageHost)?.OldPage;

            // Store the current content as old page
            var oldPageContent = newPageFrame?.Content;

            // Remove current page from new page frame
            newPageFrame.Content = null;

            // Move the previous page into the old page frame
            oldPageFrame.Content = oldPageContent;

            // Animate out previous page ... TODO pridat animacie?

            // Set the new page content
            newPageFrame.Content = e.NewValue;
        } 

        #endregion
    }
}
