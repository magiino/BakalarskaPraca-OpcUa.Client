using System;
using System.ComponentModel;
using System.Windows;
using OpcUa.Client.Core;

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
            DataContext = new WindowViewModel();
            IoC.AppManager.CloseAction = Close;
        }

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            IoC.DisposeAll();
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            ((WindowViewModel) DataContext).DimmableOverlayVisible = true;
        }

        private void MainWindow_OnActivated(object sender, EventArgs e)
        {
            ((WindowViewModel) DataContext).DimmableOverlayVisible = false;
        }
    }
}

