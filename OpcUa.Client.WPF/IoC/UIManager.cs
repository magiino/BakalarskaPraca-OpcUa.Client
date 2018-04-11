using System.Threading.Tasks;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// The applications implementation of the <see cref="IUIManager"/>
    /// </summary>
    public class UIManager : IUIManager
    {
        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowMessage(BaseDialogViewModel viewModel)
        {
            return new DialogMessageBox().ShowDialog(viewModel);
        }

        /// <summary>
        /// Displays a add notification window to user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowAddNotification(BaseDialogViewModel viewModel)
        {
            return new DialogAddNotification().ShowDialog(viewModel);
        }

        /// <summary>
        /// Displays a add notification window to user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowLogIn(BaseDialogViewModel viewModel)
        {
            return new DialogLogin().ShowDialog(viewModel);
        }

        /// <summary>
        /// Displays a add notification window to user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowOption(BaseDialogViewModel viewModel)
        {
            return new DialogDecide().ShowDialog(viewModel);
        }
    }
}
