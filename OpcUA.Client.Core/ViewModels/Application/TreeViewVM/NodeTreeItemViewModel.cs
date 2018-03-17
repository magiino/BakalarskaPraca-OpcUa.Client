using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    /// <summary>
    /// A view model for each directory item
    /// </summary>
    public class NodeTreeItemViewModel : BaseViewModel
    {
        #region Private Fields

        /// <summary>
        /// The Action for sending selected node to parent view model
        /// </summary>
        private readonly Action<ReferenceDescription> _setSelectedNode;

        private bool _isSelected;

        #endregion

        #region Public Properties

        /// <summary>
        /// A current node
        /// </summary>
        public ReferenceDescription Node { get; set; }

        /// <summary>
        /// The type of this node
        /// </summary>
        public NodeClass Type => Node.NodeClass;

        /// <summary>
        /// The name of this node
        /// </summary>
        public string Name => Node.DisplayName.ToString();

        /// <summary>
        /// A list of all children contained inside this node
        /// </summary>
        public ObservableCollection<NodeTreeItemViewModel> Children { get; set; }

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

        /// <summary>
        /// Indicates if current item is selected or not
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                if (_isSelected)
                    MessengerInstance.Send(new SendSelectedRefNode(Node));
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
        /// <param name="setSelectedNode"></param>
        public NodeTreeItemViewModel(ReferenceDescription node)
        {
            ExpandCommand = new RelayCommand(Expand);

            Node = node;

            // Setup the children as needed
            ClearChildren();
        }

        #endregion

        #region Command Methods
        /// <summary>
        ///  Expands this directory and finds all children
        /// </summary>
        private void Expand()
        {
            // We cannot expand a file
            if (Type == NodeClass.Unspecified)
                return;

            // Find all children
            var children = IoC.UaClientApi.BrowseNode(Node);
            Children = new ObservableCollection<NodeTreeItemViewModel>(
                                children.Select(content => new NodeTreeItemViewModel(content)).OrderBy(x => x.Name));
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Removes all children from the list, adding a dummy item to show the expand icon if required
        /// </summary>
        private void ClearChildren()
        {
            // Clear items
            Children = new ObservableCollection<NodeTreeItemViewModel>();

            // Show the expand arrow if we are not a file
            if (Type != NodeClass.Unspecified)
                Children.Add(null);
        }

        #endregion
    }
}
