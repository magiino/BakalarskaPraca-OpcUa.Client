using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUA.Client.Core
{
    public class UaClientApi
    {
        #region Private Fields

        /// <summary> 
        /// Specifies this application.
        /// </summary>
        private ApplicationConfiguration _applicationConfig;

        #endregion

        #region public Properties

        /// <summary>
        /// Provides the session being established with an OPC UA server.
        /// </summary>
        public Session Session { get; private set; }

        private Subscription Subscription { get; set; }

        private ICollection<MonitoredItem> myMonitoredItem = new List<MonitoredItem>();


        /// <summary>
        /// Provides the event handling for server certificates.
        /// </summary>
        public CertificateValidationEventHandler CertificateValidationNotification;

        /// <summary>
        /// Provides the event for value changes of a monitored item.
        /// </summary>
        public MonitoredItemNotificationEventHandler ItemChangedNotification;

        /// <summary>
        /// Provides the event for KeepAliveNotifications.
        /// </summary>
        public KeepAliveEventHandler KeepAliveNotification;
        #endregion

        #region Constructors

        public UaClientApi()
        {
            // Creats the application configuration (containing the certificate) on construction
            _applicationConfig = CreateClientConfiguration();
        }
        #endregion

        #region Discovery Client

        /// <summary>Finds Servers based on a discovery url</summary>
        /// <param name="discoveryUrl">The discovery url</param>
        /// <returns>ApplicationDescriptionCollection containing found servers</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ApplicationDescriptionCollection FindServers(string discoveryUrl)
        {
            //Create a URI using the discovery URL
            var uri = new Uri(discoveryUrl);
            try
            {
                //Ceate a DiscoveryClient
                using (var client = DiscoveryClient.Create(uri))
                {
                    //Find servers
                    var servers = client.FindServers(null);
                    return servers;
                }
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Finds Endpoints based on a server's url</summary>
        /// <param name="serverUrl">The server's url for discovery endpoints</param>
        /// <returns>EndpointDescriptionCollection containing found Endpoints</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public EndpointDescriptionCollection GetEndpoints(string serverUrl)
        {
            //Create a URI using the server's URL
            var uri = new Uri(serverUrl);
            try
            {
                //Create a DiscoveryClient
                using (var discoveryClient = DiscoveryClient.Create(uri))
                {
                    //Search for available endpoints
                    var endpoints = discoveryClient.GetEndpoints(null);
                    return endpoints;
                }
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        #endregion

        #region Public Methods

        public void CreateDefaultConfiguration()
        {
            _applicationConfig = CreateClientConfiguration();
        }

        public void SaveConfiguration()
        {
            _applicationConfig.SaveToFile("C:\\Users\\Marek\\UaClient\\test.xml");
        }

        #endregion

        #region Browse
        /// <summary>Browses the root folder of an OPC UA server.</summary>
        /// <returns>ReferenceDescriptionCollection of found nodes</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ReferenceDescriptionCollection BrowseRoot()
        {
            try
            {
                //Browse the RootFolder for variables, objects and methods
                Session.Browse(null, null, ObjectIds.RootFolder, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out var continuationPoint, out var referenceDescriptionCollection);
                return referenceDescriptionCollection;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }

        }

        /// <summary>Browses a node ID provided by a ReferenceDescription</summary>
        /// <param name="refDesc">The ReferenceDescription</param>
        /// <returns>ReferenceDescriptionCollection of found nodes</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ReferenceDescriptionCollection BrowseNode(ReferenceDescription refDesc)
        {
            //Create a collection for the browse results
            ReferenceDescriptionCollection referenceDescriptionCollection;
            //Create a continuationPoint
            byte[] continuationPoint;
            //Create a NodeId using the selected ReferenceDescription as browsing starting point
            NodeId nodeId = ExpandedNodeId.ToNodeId(refDesc.NodeId, null);
            try
            {
                //Browse from starting point for all object types
                Session.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out continuationPoint, out referenceDescriptionCollection);
                return referenceDescriptionCollection;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }

        }
        #endregion

        /// <summary>Establishes the connection to an OPC UA server and creates a session using an EndpointDescription.</summary>
        /// <param name="endpointDescription">The EndpointDescription of the server's endpoint</param>
        /// <param name="userAuth">Autheticate anonymous or with username and password</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Connect(EndpointDescription endpointDescription, bool userAuth, string userName, string password)
        {
            try
            {
                //Secify application configuration
                var applicationConfig = _applicationConfig;

                //Hook up a validator function for a CertificateValidation event
                applicationConfig.CertificateValidator.CertificateValidation += Notificatio_CertificateValidation;

                //Create EndPoint configuration
                var endpointConfiguration = EndpointConfiguration.Create(applicationConfig);

                //Connect to server and get endpoints
                var mEndpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                //Create the binding factory.
                //var bindingFactory = BindingFactory.Create(_applicationConfig, ServiceMessageContext.GlobalContext);

                //Creat a session name
                String sessionName = "MySession";

                //Create user identity
                var userIdentity = userAuth ? new UserIdentity(userName, password) : new UserIdentity();

                //Update certificate store before connection attempt
                applicationConfig.CertificateValidator.Update(applicationConfig);

                //Create and connect session
                Session = Session.Create(
                    applicationConfig,
                    mEndpoint,
                    true,
                    sessionName,
                    60000,
                    userIdentity,
                    null
                    );

                Session.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Closes an existing session and disconnects from the server.</summary>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Disconnect()
        {
            // Close the session.
            if (Session == null) return;
            try
            {
                Session.Close(10000);
                Session.Dispose();
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }


        #region Subscription
        /// <summary>Creats a Subscription object to a server</summary>
        /// <param name="publishingInterval">The publishing interval</param>
        /// <returns>Subscription</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Subscribe(int publishingInterval)
        {
            //Create a Subscription object
            var subscription = new Subscription(Session.DefaultSubscription)
            {
                //Enable publishing
                PublishingEnabled = true,
                //Set the publishing interval
                PublishingInterval = publishingInterval
            };
            //Add the subscription to the session
            Session.AddSubscription(subscription);
            try
            {
                //Create/Activate the subscription
                subscription.Create();
                
                //ItemChangedNotification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);

                Subscription = subscription;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Ads a monitored item to an existing subscription</summary>
        /// <param name="node"></param>
        /// <param name="samplingInterval">The sampling interval</param>
        /// <returns>The added item</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public MonitoredItem AddMonitoredItem(NodeId node, int samplingInterval)
        {
            //Create a monitored item
            MonitoredItem monitoredItem = new MonitoredItem();
            //Set the name of the item for assigning items and values later on; make sure item names differ
            monitoredItem.DisplayName = node.ToString();
            //Set the NodeId of the item
            monitoredItem.StartNodeId = node.ToString();
            //Set the attribute Id (value here)
            monitoredItem.AttributeId = Attributes.Value;
            //Set reporting mode
            monitoredItem.MonitoringMode = MonitoringMode.Reporting;
            //Set the sampling interval (1 = fastest possible)
            monitoredItem.SamplingInterval = samplingInterval;
            //Set the queue size
            monitoredItem.QueueSize = 1;
            //Discard the oldest item after new one has been received
            monitoredItem.DiscardOldest = true;
            //Define event handler for this item and then add to monitoredItem
            //monitoredItem.Notification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);
            try
            {
                //Add the item to the subscription
                Subscription.AddItem(monitoredItem);
                //Apply changes to the subscription
                Subscription.ApplyChanges();

                return monitoredItem;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Removs a monitored item from an existing subscription</summary>
        /// <param name="subscription">The subscription</param>
        /// <param name="monitoredItem">The item</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public MonitoredItem RemoveMonitoredItem(Subscription subscription, MonitoredItem monitoredItem)
        {
            try
            {
                //Add the item to the subscription
                subscription.RemoveItem(monitoredItem);
                //Apply changes to the subscription
                subscription.ApplyChanges();
                return null;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Removes an existing Subscription</summary>
        /// <param name="subscription">The subscription</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void RemoveSubscription(Subscription subscription)
        {
            try
            {
                //Delete the subscription and all items submitted
                subscription.Delete(true);
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }
        #endregion



        #region Read/Write
        /// <summary>Reads a node by node Id</summary>
        /// <param name="nodeIdString">The node Id as string</param>
        /// <returns>The read node</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Node ReadNode(String nodeIdString)
        {
            //Create a nodeId using the identifier string
            NodeId nodeId = new NodeId(nodeIdString);
            //Create a node
            Node node = new Node();
            try
            {
                //Read the dataValue
                node = Session.ReadNode(nodeId);
                return node;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Reads values from node Ids</summary>
        /// <param name="nodeIdStrings">The node Ids as strings</param>
        /// <returns>The read values as strings</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public List<string> ReadValues(List<String> nodeIdStrings)
        {
            List<NodeId> nodeIds = new List<NodeId>();
            List<Type> types = new List<Type>();
            List<object> values = new List<object>();
            List<ServiceResult> serviceResults = new List<ServiceResult>();
            foreach (string str in nodeIdStrings)
            {
                //Create a nodeId using the identifier string and add to list
                nodeIds.Add(new NodeId(str));
                //No need for types
                types.Add(null);
            }
            try
            {
                //Read the dataValues
                Session.ReadValues(nodeIds, types, out values, out serviceResults);
                //check ServiceResults to 
                foreach (ServiceResult svResult in serviceResults)
                {
                    if (svResult.ToString() != "Good")
                    {
                        Exception e = new Exception(svResult.ToString());
                        throw e;
                    }
                }
                List<string> resultStrings = new List<string>();
                foreach (object result in values)
                {
                    if (result != null)
                    {
                        if (result.ToString() == "System.Byte[]")
                        {
                            string str = "";
                            str = BitConverter.ToString((byte[])result).Replace("-", ";");
                            resultStrings.Add(str);
                        }
                        if (result.ToString() == "System.String[]")
                        {
                            string str = "";
                            str = String.Join(";", (string[])result);
                            resultStrings.Add(str);
                        }
                        else if (result.ToString() == "System.Boolean[]")
                        {
                            string str = "";
                            foreach (Boolean intVar in (Boolean[])result)
                            {
                                str = str + ";" + intVar.ToString();
                            }
                            str = str.Remove(0, 1);
                            resultStrings.Add(str);
                        }
                        else if (result.ToString() == "System.Int16[]")
                        {
                            string str = "";
                            foreach (Int16 intVar in (Int16[])result)
                            {
                                str = str + ";" + intVar.ToString();
                            }
                            str = str.Remove(0, 1);
                            resultStrings.Add(str);
                        }
                        else if (result.ToString() == "System.UInt16[]")
                        {
                            string str = "";
                            foreach (UInt16 intVar in (UInt16[])result)
                            {
                                str = str + ";" + intVar.ToString();
                            }
                            str = str.Remove(0, 1);
                            resultStrings.Add(str);
                        }
                        else if (result.ToString() == "System.Int64[]")
                        {
                            string str = "";
                            foreach (Int64 intVar in (Int64[])result)
                            {
                                str = str + ";" + intVar.ToString();
                            }
                            str = str.Remove(0, 1);
                            resultStrings.Add(str);
                        }
                        else if (result.ToString() == "System.Single[]")
                        {
                            string str = "";
                            foreach (float intVar in (float[])result)
                            {
                                str = str + ";" + intVar.ToString();
                            }
                            str = str.Remove(0, 1);
                            resultStrings.Add(str);
                        }
                        else if (result.ToString() == "System.Double[]")
                        {
                            string str = "";
                            foreach (double intVar in (double[])result)
                            {
                                str = str + ";" + intVar.ToString();
                            }
                            str = str.Remove(0, 1);
                            resultStrings.Add(str);
                        }
                        else
                        {
                            resultStrings.Add(result.ToString());
                        }
                    }
                    else
                    {
                        resultStrings.Add("(null)");
                    }
                }
                return resultStrings;
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }

        /// <summary>Writes values to node Ids</summary>
        /// <param name="value">The values as strings</param>
        /// <param name="nodeIdString">The node Ids as strings</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void WriteValues(List<String> values, List<String> nodeIdStrings)
        {
            //Create a collection of values to write
            WriteValueCollection valuesToWrite = new WriteValueCollection();
            //Create a collection for StatusCodes
            StatusCodeCollection result = new StatusCodeCollection();
            //Create a collection for DiagnosticInfos
            DiagnosticInfoCollection diagnostics = new DiagnosticInfoCollection();

            foreach (String str in nodeIdStrings)
            {
                //Create a nodeId
                NodeId nodeId = new NodeId(str);
                //Create a dataValue
                DataValue dataValue = new DataValue();
                //Read the dataValue
                try
                {
                    dataValue = Session.ReadValue(nodeId);
                }
                catch (Exception e)
                {
                    //handle Exception here
                    throw e;
                }

                //Get the data type of the read dataValue
                //Handle Arrays here: TBD
                Variant variant = new Variant(Convert.ChangeType(values[nodeIdStrings.IndexOf(str)], dataValue.Value.GetType()));

                //Overwrite the dataValue with a new constructor using read dataType
                dataValue = new DataValue(variant);

                //Create a WriteValue using the NodeId, dataValue and attributeType
                WriteValue valueToWrite = new WriteValue();
                valueToWrite.Value = dataValue;
                valueToWrite.NodeId = nodeId;
                valueToWrite.AttributeId = Attributes.Value;

                //Add the dataValues to the collection
                valuesToWrite.Add(valueToWrite);
            }
            try
            {
                //Write the collection to the server
                Session.Write(null, valuesToWrite, out result, out diagnostics);
            }
            catch (Exception e)
            {
                //handle Exception here
                throw e;
            }
        }
        #endregion




        #region EventHandling
        /// <summary>Eventhandler to validate the server certificate forwards this event</summary>
        private void Notificatio_CertificateValidation(CertificateValidator certificate, CertificateValidationEventArgs e)
        {
            CertificateValidationNotification(certificate, e);
        }

        /*
        /// <summary>Eventhandler for MonitoredItemNotifications forwards this event</summary>
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            ItemChangedNotification(monitoredItem, e);
        }
        */
        /// <summary>Eventhandler for KeepAlive forwards this event</summary>
        private void Notification_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            if (e != null)
            {
                KeepAliveNotification(session, e);
            }
        }
        #endregion

        #region Private methods

        /// <summary>Creats a minimal required ApplicationConfiguration</summary>
        /// <returns>The ApplicationConfiguration</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        private static ApplicationConfiguration CreateClientConfiguration()
        {
            // The application configuration can be loaded from any file.
            // ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
            // This approach allows applications to share configuration files and to update them.
            // This example creates a minimum ApplicationConfiguration using its default constructor.
            ApplicationConfiguration configuration = new ApplicationConfiguration();

            // Step 1 - Specify the client identity.
            configuration.ApplicationName = "UaClientApi";
            configuration.ApplicationType = ApplicationType.Client;
            configuration.ApplicationUri = "urn:UaClientApi"; //Kepp this syntax
            configuration.ProductUri = "SiemensAG.IndustryOnlineSupport";

            // Step 2 - Specify the client's application instance certificate.
            // Application instance certificates must be placed in a windows certficate store because that is 
            // the best way to protect the private key. Certificates in a store are identified with 4 parameters:
            // StoreLocation, StoreName, SubjectName and Thumbprint.
            // When using StoreType = Directory you need to have the opc.ua.certificategenerator.exe installed on your machine

            configuration.SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "CurrentUser\\My",
                    SubjectName = configuration.ApplicationName
                },
                TrustedIssuerCertificates =
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "CurrentUser\\Root"
                },
                TrustedPeerCertificates =
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "CurrentUser\\Root"
                }
            };

            // find the client certificate in the store.
            var clientCertificate = configuration.SecurityConfiguration.ApplicationCertificate.Find(true);

            // create a new self signed certificate if not found.
            if (clientCertificate == null)
            {
                // Get local interface ip addresses and DNS name
                List<string> localIps = GetLocalIpAddressAndDns();

                UInt16 keySize = 2048; //must be multiples of 1024
                UInt16 lifeTime = 24; //in month
                UInt16 algorithm = 1; //0 = SHA1; 1 = SHA256

                // this code would normally be called as part of the installer - called here to illustrate.
                // create a new certificate an place it in the current user certificate store.
                clientCertificate = CertificateFactory.CreateCertificate(
                    configuration.SecurityConfiguration.ApplicationCertificate.StoreType,
                    configuration.SecurityConfiguration.ApplicationCertificate.StorePath,
                    configuration.ApplicationUri,
                    configuration.ApplicationName,
                    null,
                    localIps,
                    keySize,
                    lifeTime,
                    algorithm);
            }

            // Step 3 - Specify the supported transport quotas.
            // The transport quotas are used to set limits on the contents of messages and are
            // used to protect against DOS attacks and rogue clients. They should be set to
            // reasonable values.
            configuration.TransportQuotas = new TransportQuotas
            {
                OperationTimeout = 360000,
                MaxStringLength = 67108864,
                MaxByteStringLength = 16777216
            };
            //Needed, i.e. for large TypeDictionarys


            // Step 4 - Specify the client specific configuration.
            configuration.ClientConfiguration = new ClientConfiguration();
            configuration.ClientConfiguration.DefaultSessionTimeout = 360000;



            // Add trace config before calling validate
            configuration.TraceConfiguration = new TraceConfiguration
            {
                OutputFilePath = "tracelog.txt",
                DeleteOnLoad = true,
                TraceMasks = Utils.TraceMasks.All
            };

            // Step 5 - Validate the configuration.
            // This step checks if the configuration is consistent and assigns a few internal variables
            // that are used by the SDK. This is called automatically if the configuration is loaded from
            // a file using the ApplicationConfiguration.Load() method.
            configuration.Validate(ApplicationType.Client);

            return configuration;
        }

        /// <summary>Creats an EndpointDescription</summary>
        /// <param name="url">The endpoint url</param>
        /// <param name="security">Use security or not</param>
        /// <returns>The EndpointDescription</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        private static EndpointDescription CreateEndpointDescription(string url, string secPolicy, MessageSecurityMode msgSecMode)
        {
            // create the endpoint description.
            EndpointDescription endpointDescription = new EndpointDescription();

            // submit the url of the endopoint
            endpointDescription.EndpointUrl = url;

            // specify the security policy to use.

            endpointDescription.SecurityPolicyUri = secPolicy;
            endpointDescription.SecurityMode = msgSecMode;

            // specify the transport profile.
            endpointDescription.TransportProfileUri = Profiles.UaTcpTransport;

            return endpointDescription;
        }

        /// <summary>Gets the local IP addresses and the DNS name</summary>
        /// <returns>The list of IPs and names</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        private static List<string> GetLocalIpAddressAndDns()
        {
            List<string> localIps = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIps.Add(ip.ToString());
                }
            }
            if (localIps.Count == 0)
            {
                throw new Exception("Local IP Address Not Found!");
            }
            localIps.Add(Dns.GetHostName());
            return localIps;
        }

        #endregion
    }
}