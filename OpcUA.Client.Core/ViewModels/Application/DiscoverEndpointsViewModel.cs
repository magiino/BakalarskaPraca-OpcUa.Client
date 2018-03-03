using System.Windows.Input;

namespace OpcUA.Client.Core
{
    public class DiscoverEndpointsViewModel : BaseViewModel
    {
        #region Public Properties

        #endregion

        #region Commands

        /// <summary>
        /// The command for search endpoints
        /// </summary>
        public ICommand SearchCommand { get; set; }

        #endregion

        #region Constructor

        public DiscoverEndpointsViewModel()
        {
            SearchCommand = new RelayCommand(SearchEndpoints);
        } 

        #endregion

        private void SearchEndpoints()
        {

        }
    }
}
