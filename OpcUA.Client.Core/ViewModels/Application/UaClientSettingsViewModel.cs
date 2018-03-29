using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class UaClientSettingsViewModel : BaseViewModel
    {
        #region Public Properties

        #endregion

        #region Commands

        /// <summary>
        /// The command for search endpoints
        /// </summary>
        public ICommand DoneCommand { get; set; }

        #endregion

        #region Constructor

        public UaClientSettingsViewModel()
        {
            DoneCommand = new RelayCommand(SetUpDone);
        } 

        #endregion

        private void SetUpDone()
        {

        }
    }
}
