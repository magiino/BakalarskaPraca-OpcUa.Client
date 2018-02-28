using System;
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

        private IsSelected IsSelectedDelegate { get; }

        /// <summary>
        /// The type of this node
        /// </summary>
        public NodeClass Type => Node.NodeClass;

        /// <summary>
        /// The name of this node
        /// </summary>
        public string Name => Node.DisplayName.ToString();

        /// <summary>
        /// A current node
        /// </summary>
        public ReferenceDescription Node { get; set; }

        /// <summary>
        /// A list of all children contained inside this node
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
            get => Children?.Count(f => f != null) > 0;
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

        public ReferenceDescription SelectedNode { get; set; }


        /// <summary>
        /// Indicates if current item is selected or not
        /// </summary>
        public bool IsSelected
        {
            get => SelectedNode != null;
            set
            {
                if (!value)
                {
                    SelectedNode = null;
                    return;
                }
                SelectedNode = Node;
                IsSelectedDelegate(SelectedNode);
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
        public NodeItemViewModel(ReferenceDescription node, IsSelected nodeIsSelected)
        {
            // Create commands
            ExpandCommand = new RelayCommand(Expand);
            // Set delegate
            IsSelectedDelegate = nodeIsSelected;
            // Set node item
            Node = node;
            // Setup the children as needed
            ClearChildren();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        private void ClearChildren()
        {
            // Clear items
            Children = new ObservableCollection<NodeItemViewModel>();

            // Show the expand arrow if we are not a file
            if (Type != NodeClass.Unspecified)
                Children.Add(null);
        }

        #endregion

        /// <summary>
        ///  Expands this directory and finds all children
        /// </summary>
        private void Expand()
        {
            // We cannot expand a file
            if (Type == NodeClass.Unspecified)
                return;

            // Find all children
            var children = IoC.Get<UAClientHelperAPI>().BrowseNode(Node);
            Children = new ObservableCollection<NodeItemViewModel>(
                                children.Select(content => new NodeItemViewModel(content, IsSelectedDelegate)));
        }
    }
}
