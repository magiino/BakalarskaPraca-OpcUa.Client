using System.Threading.Tasks;

namespace OpcUa.Client.Core
{
    /// <summary>
    /// The UI manager that handles any UI interaction in the application
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowMessage(BaseDialogViewModel viewModel);

        /// <summary>
        /// Displays a add notification window to user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowAddNotification(BaseDialogViewModel viewModel);

        /// <summary>
        /// Displays a log in window to user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowLogIn(BaseDialogViewModel viewModel);

        /// <summary>
        /// Displays a option window to user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowOption(BaseDialogViewModel viewModel);
    }
}
