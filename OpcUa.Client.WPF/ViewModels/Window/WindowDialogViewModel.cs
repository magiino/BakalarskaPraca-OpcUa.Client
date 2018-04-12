using System.Windows.Controls;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class WindowDialogViewModel : WindowViewModel
    {
        #region Public Properties
        public string Title { get; set; }
        public Control Content { get; set; }
        #endregion

        #region Constructor
        public WindowDialogViewModel()
        {
            WindowMinimumWidth = 250;
            WindowMinimumHeight = 100;
        }
        #endregion
    }
}