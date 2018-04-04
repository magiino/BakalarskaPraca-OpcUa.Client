using System.Linq;
using System.Windows.Controls;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for LiveCharts.xaml
    /// </summary>
    public partial class ZoomChart : UserControl
    {
        public ZoomChart()
        {
            InitializeComponent();
            var b = Y;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewmodel = (ZoomChartViewModel)DataContext;
            viewmodel.SelectedVariables = Variables.SelectedItems.Cast<VariableEntity>().ToList();
        }
    }
}
