using System.Windows.Input;

namespace OpcUA.Client.Core
{
    /// <summary>
    /// The view model for the address space tab item
    /// </summary>
    public class NodeAttributesViewModel : BaseViewModel
    {
        #region Public Properties


        #endregion

        #region Commands

        /// <summary>
        /// The command for exit application
        /// </summary>
        public ICommand AddVariableCommand { get; set; } 

        #endregion

        #region Constructor

        public NodeAttributesViewModel()
        {
            AddVariableCommand = new RelayCommand(Add);

        }

        private void Add()
        {
           
        }

        #endregion
    }
}
