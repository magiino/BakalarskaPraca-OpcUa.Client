using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Siemens.UAClientHelper;
using Opc.Ua;
using Opc.Ua.Client;

namespace OPC_UA_client_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members
        //private UAClientHelperAPI myClientHelperAPI;
        //private Session mySession;
        //private Subscription mySubscription;
        //private ReferenceDescriptionCollection referenceDescriptionCollection;

        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new DirectoryStructureViewModel();

            //myClientHelperAPI = new UAClientHelperAPI();
            //Connect();
        }

        #endregion
    }
}



/*
private void Connect()
{
    if (mySession != null && !mySession.Disposed)
    {
        try
        {
            mySubscription.Delete(true);
        }
        catch
        {
            ;
        }

        myClientHelperAPI.Disconnect();
        mySession = myClientHelperAPI.Session;
    }
    else
    {
        try
        {

            EndpointDescription mySelectedEndpoint = new EndpointDescription("opc.tcp://A05-226b:53530/OPCUA/SimulationServer");

            myClientHelperAPI.Connect(mySelectedEndpoint, false, "", "");
            mySession = myClientHelperAPI.Session;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }
}
*/
