using System.Windows;
using OpcUA.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureIoc();

            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }

        private static void ConfigureIoc()
        {
            // Configure IoC
            IoC.Configure();
        }
    }
}
