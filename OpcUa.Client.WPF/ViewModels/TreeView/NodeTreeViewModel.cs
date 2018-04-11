using System.Collections.ObjectModel;
using System.Linq;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// The view model for the address space tab item
    /// </summary>
    public class NodeTreeViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// A list of root nodes in the address space
        /// </summary>
        public ObservableCollection<NodeTreeItemViewModel> Items { get; set; }

        #endregion

        #region Constructor

        public NodeTreeViewModel()
        {
            //TODO nepouzivat tu IOC?
            // Get the root nodes
            var children = IoC.UaClientApi.BrowseRoot();
            
            //Create the view models from the root ndoes
            Items = new ObservableCollection<NodeTreeItemViewModel>(
                children.Select(content => new NodeTreeItemViewModel(content)).OrderBy(x => x.Name));
        }

        #endregion
    }
}
