using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    /// <inheritdoc />
    /// <summary>
    /// A view model for each directory item
    /// </summary>
    public class NodeItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The type of this item
        /// </summary>
        public NodeClass Type => Node.NodeClass;

        public string ImageName => Node.NodeClass.ToString();
        // TODO NodeClass

        /// <summary>
        /// A current node
        /// </summary>
        public ReferenceDescription Node { get; set; }

        /// <summary>
        /// The name of this directory item
        /// </summary>
        public string Name => Node.DisplayName.ToString();

        /// <summary>
        /// A list of all children contained inside this item
        /// </summary>
        public ObservableCollection<NodeItemViewModel> Children { get; set; }

        /// <summary>
        /// Indicates if this item can be expanded
        /// </summary>
        public bool CanExpand => Type != NodeClass.Unspecified;

        /// <summary>
        /// Indicates if the current item is expanded or not
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return Children?.Count(f => f != null) > 0;
            }
            set
            {
                // If the UI tells us to expand...
                if (value)
                    // Find all children
                    Expand();
                // If the UI tells us to close
                else
                    ClearChildren();
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="node"></param>
        public NodeItemViewModel(ReferenceDescription node)
        {
            // Create commands
            this.ExpandCommand = new RelayCommand(Expand);

            // Set path and type
            //this.FullPath = fullPath;
            this.Node = node;

            // Setup the children as needed
            this.ClearChildren();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        private void ClearChildren()
        {
            // Clear items
            this.Children = new ObservableCollection<NodeItemViewModel>();

            // Show the expand arrow if we are not a file
            if (this.Type != NodeClass.Unspecified)
                this.Children.Add(null);
        }

        #endregion

        /// <summary>
        ///  Expands this directory and finds all children
        /// </summary>
        private void Expand()
        {
            // We cannot expand a file
            if (this.Type == NodeClass.Unspecified)
                return;

            // Find all children
            var children = IoC.Get<UAClientHelperAPI>().BrowseNode(this.Node);
            this.Children = new ObservableCollection<NodeItemViewModel>(
                                children.Select(content => new NodeItemViewModel(content)));
        }
    }
}
