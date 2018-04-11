using System.Windows;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureIoC();

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }

        private static void ConfigureIoC()
        {
            // Configure IoC
            IoC.Configure();

            IoC.Kernel.Bind<IUIManager>().ToConstant(new UIManager());

            // Set up mappers for charts globally

            var dateTimePointCfg = Mappers.Xy<DateTimePoint>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);
            Charting.For<DateTimePoint>(dateTimePointCfg);


            var measureModelCfg = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);
            Charting.For<MeasureModel>(measureModelCfg);
        }
    }
}