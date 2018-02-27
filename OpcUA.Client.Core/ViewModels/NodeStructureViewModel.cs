using System.Collections.ObjectModel;
using System.Linq;

namespace OpcUA.Client.Core
{
    /// <inheritdoc />
    /// <summary>
    /// The view model for the applications main Directory view
    /// </summary>
    public class NodeStructureViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A list of all directories on the machine
        /// </summary>
        public ObservableCollection<NodeItemViewModel> Items { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NodeStructureViewModel()
        {
            // Get the logical drives
            var children = IoC.Get<UAClientHelperAPI>().BrowseRoot();

            // Create the view models from the data
            Items = new ObservableCollection<NodeItemViewModel>(
                children.Select(content => new NodeItemViewModel(content)));
        }

        #endregion
    }
}
