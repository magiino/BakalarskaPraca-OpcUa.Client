using System;
using System.ComponentModel;
using System.Windows;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUA.Client.Core;

namespace OpcUA.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members
        private Session _mySession;

        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IoC.UaClient.Disconnect();
        }

        #endregion
        private void Connect()
        {
            var client = IoC.UaClient;
            if (_mySession != null && !_mySession.Disposed)
            {

                client.Disconnect();
                _mySession = IoC.UaClient.Session;
            }
            else
            {
                try
                {
                    EndpointDescription mySelectedEndpoint = new EndpointDescription("opc.tcp://A05-226b:48010");

                    client.Connect(mySelectedEndpoint, false, "", "");
                    _mySession = client.Session;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }
    }
}

