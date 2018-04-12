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

            IoC.Messenger.Register<SendResetAxises>((msg) =>
            {
                X.MinValue = double.NaN;
                X.MaxValue = double.NaN;
                Y.MinValue = double.NaN;
                Y.MaxValue = double.NaN;
            });
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewmodel = (ZoomChartViewModel)DataContext;
            viewmodel.SelectedVariables = Variables.SelectedItems.Cast<VariableEntity>().ToList();
        }
    }
}
