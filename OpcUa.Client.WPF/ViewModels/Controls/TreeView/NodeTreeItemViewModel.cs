using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Opc.Ua;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// A view model for each directory item
    /// </summary>
    public class NodeTreeItemViewModel : BaseViewModel
    {
        #region Private Fields
        private bool _isSelected;
        #endregion

        #region Public Properties
        public ReferenceDescription Node { get; set; }
        public NodeClass Type => Node.NodeClass;
        public string Name => Node.DisplayName.ToString();
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
                    Expand(null);
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
                    IoC.Messenger.Send(new SendSelectedRefNode(Node));
            }
        }
        #endregion

        #region Public Commands
        /// <summary>
        /// The command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; }
        #endregion

        #region Constructor
        public NodeTreeItemViewModel(ReferenceDescription node)
        {
            ExpandCommand = new MixRelayCommand(Expand);
            Node = node;

            // Setup the children as needed
            ClearChildren();
        }
        #endregion

        #region Command Methods
        /// <summary>
        ///  Expands this item in address space and finds all children
        /// </summary>
        private void Expand(object parameter)
        {
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