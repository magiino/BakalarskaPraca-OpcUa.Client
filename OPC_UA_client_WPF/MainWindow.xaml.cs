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
            //myClientHelperAPI = new UAClientHelperAPI();
            //Connect();
        }

        #endregion

        #region On Loaded
        /// <summary>
        /// When the application first opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            if (referenceDescriptionCollection == null)
            {
                try
                {
                    referenceDescriptionCollection = myClientHelperAPI.BrowseRoot();
                    foreach (ReferenceDescription refDesc in referenceDescriptionCollection)
                    {
                        var item = new TreeViewItem();
                        item.Header = refDesc.DisplayName;
                        NodeView.Items.Add(item);
                        nodeTreeView.Nodes.Add(refDesc.DisplayName.ToString()).Tag = refDesc;
                        foreach (TreeNode node in nodeTreeView.Nodes)
                        {
                            node.Nodes.Add("");
                        }  
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            */
            // Get every logical drive on the machine
            foreach (var drive in Directory.GetLogicalDrives())
            {
                // Create a new item for it
                var item = new TreeViewItem()
                {
                    // Set the header
                    Header = drive,
                    // And the full path
                    Tag = drive
                };

                // Dummy item
                item.Items.Add(null);

                // Listen out for item being expanded
                item.Expanded += Folder_Expanded;

                // Add it to the main tree-view
                NodeView.Items.Add(item);
            } 
        }

        #endregion

        #region File Expanded
        /// <summary>
        /// When a folder is expanded, find the sub folders/files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initial Checks
            var item = (TreeViewItem)sender;

            // If the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            // Clear dummy data
            item.Items.Clear();

            // Get full path
            var fullPath = (string)item.Tag;

            #endregion

            #region Get Folders
            // Create a blank list for directories
            var directories = new List<string>();

            // Try and get directories from the folder
            // ignoring any issues doing so
            try
            {
                var dirs = Directory.GetDirectories(fullPath);
                
                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }

            directories.ForEach(directoryPath =>
            {
                // Create directory item
                var subItem = new TreeViewItem()
                {
                    // Set header as folder name
                    Header = GetFileFolderName(directoryPath),
                    // And tag as full path
                    Tag = directoryPath
                };

                // Add dummy item so we can expand folder
                subItem.Items.Add(null);

                // Handle expanding
                subItem.Expanded += Folder_Expanded;

                // Add this folder to the parent
                item.Items.Add(subItem);

            });
            #endregion

            #region Get Files
            // Create a blank list for files
            var files = new List<string>();

            // Try and get files from the folder
            // ignoring any issues doing so
            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            files.ForEach(filePath =>
            {
                // Create file item
                var subItem = new TreeViewItem()
                {
                    // Set header as file name
                    Header = GetFileFolderName(filePath),
                    // And tag as full path
                    Tag = filePath
                };

                // Add this folder to the parent
                item.Items.Add(subItem);
            });

            #endregion

        }

        #endregion

        #region Helpers
        /// <summary>
        /// Find the file or folder name from full path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Make all slashes back slashes
            var normalizedPath = path.Replace('/','\\');

            // Find the last backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            // If we don`t find a backslash, return path itself
            if (lastIndex <= 0)
                return path;

            // Return the name after the last back slash
            return path.Substring(lastIndex + 1);
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
