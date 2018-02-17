using System.Windows;

namespace OpcUA.Client
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
            //this.DataContext = new DirectoryStructureViewModel();

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
