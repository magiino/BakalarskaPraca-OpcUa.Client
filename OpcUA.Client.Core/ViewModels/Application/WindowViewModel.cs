namespace OpcUa.Client.Core
{
    public class WindowViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The smallest width the window can go to
        /// </summary>
        public double WindowMinimumWidth { get; set; } = 800;

        /// <summary>
        /// The smallest height the window can go to
        /// </summary>
        public double WindowMinimumHeight { get; set; } = 500;

        /// <summary>
        /// True if main window is not focused
        /// </summary>
        public bool DimmableOverlayVisible { get; set; }

        #endregion
    }
}
