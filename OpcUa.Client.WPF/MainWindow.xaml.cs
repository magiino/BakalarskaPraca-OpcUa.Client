using System.ComponentModel;
using System.Windows;
using OpcUA.Client.Core;

namespace OpcUa.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            IoC.DisposeAll();
        }
 
    }
}

