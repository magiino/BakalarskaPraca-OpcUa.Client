using System.Windows.Controls;
using OpcUA.Client.Core;

namespace OpcUA.Client
{
    /// <summary>
    /// Interaction logic for TreeView.xaml
    /// </summary>
    public partial class TreeView : UserControl
    {
        public TreeView()
        {
            InitializeComponent();
            this.DataContext = new NodeStructureViewModel();
        }
    }
}
