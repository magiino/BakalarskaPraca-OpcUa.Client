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

        private static CertificateValidator m_validator;

        #endregion

        #region public Properties

        /// <summary>
        /// Provides the session being established with an OPC UA server.
        /// </summary>
        public Session Session { get; private set; }

        private Subscription Subscription { get; set; }

        private ICollection<MonitoredItem> myMonitoredItem = new List<MonitoredItem>();

        #endregion

        public Node GetDataType(NodeId nodeId)
        {
            var node = Session.ReadNode(nodeId);
            var dataTypeNodeId = (node.DataLock as VariableNode)?.DataType;
            var dataTypeNode = Session.ReadNode(dataTypeNodeId);
            return dataTypeNode;
        }


        #region Constructors

        public UaClientApi()
        {
            // Creats the application configuration (containing the certificate) on construction
            _applicationConfig = CreateClientConfiguration();
            CheckApplicationInstanceCertificate(false, 2048);
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

                //Session.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);
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
                Session.RemoveSubscription(subscription);
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
        
        #endregion




        #region EventHandling
        /// <summary>Eventhandler to validate the server certificate forwards this event</summary>
        private static void Notificatio_CertificateValidation(CertificateValidator certificate, CertificateValidationEventArgs e)
        {
            try
            {
                if (e.Error != null && e.Error.Code == StatusCodes.BadCertificateUntrusted)
                {
                    e.Accept = true;
                    Utils.Trace((int)Utils.TraceMasks.Security, "Automatically accepted certificate: {0}", e.Certificate.Subject);
                }
            }
            catch (Exception exception)
            {
                Utils.Trace(exception, "Error accepting certificate.");
            }

            // TODO save to trusted store
            /*
            // Always accept
            e.Accept = true;
            // Always save to store
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            if (!store.Certificates.Contains(e.Certificate))
                store.Add(e.Certificate);
                */
        }

        #endregion

        #region Private methods

        /// <summary>Creats a minimal required ApplicationConfiguration</summary>
        /// <returns>The ApplicationConfiguration</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        private ApplicationConfiguration CreateClientConfiguration()
        {
            // The application configuration can be loaded from any file.
            // ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
            // This approach allows applications to share configuration files and to update them.
            // This example creates a minimum ApplicationConfiguration using its default constructor.
            ApplicationConfiguration configuration = new ApplicationConfiguration
            {
                ApplicationName = "UaClient",
                ApplicationType = ApplicationType.Client,
                ApplicationUri = "urn:UaClient",
                ProductUri = "VutBrno.SchoolProject"
            };

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
                    SubjectName = configuration.ApplicationName,
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

           

            /*
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
            */




            // Step 3 - Specify the supported transport quotas.
            // The transport quotas are used to set limits on the contents of messages and are
            // used to protect against DOS attacks and rogue clients. They should be set to
            // reasonable values.
            configuration.TransportQuotas = new TransportQuotas
            {
                OperationTimeout = 360000,
                MaxStringLength = 67108864,
                MaxByteStringLength = 16777216 // Needed, i.e. for large TypeDictionarys
            };
            


            // Step 4 - Specify the client specific configuration.
            configuration.ClientConfiguration = new ClientConfiguration
            {
                DefaultSessionTimeout = 360000
            };



            // Add trace config before calling validate
            configuration.TraceConfiguration = new TraceConfiguration
            {
                OutputFilePath = "tracelog.txt",
                DeleteOnLoad = true,
                TraceMasks = Utils.TraceMasks.All
            };

            configuration.CertificateValidator = new CertificateValidator();

            configuration.CertificateValidator.Update(configuration.SecurityConfiguration);
            configuration.CertificateValidator.CertificateValidation += Notificatio_CertificateValidation;

            m_validator = configuration.CertificateValidator;
            ServicePointManager.ServerCertificateValidationCallback = HttpsCertificateValidation;

            // Step 5 - Validate the configuration.
            // This step checks if the configuration is consistent and assigns a few internal variables
            // that are used by the SDK. This is called automatically if the configuration is loaded from
            // a file using the ApplicationConfiguration.Load() method.
            configuration.Validate(ApplicationType.Client);

            return configuration;
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


        /// <summary>
        /// Remotes the certificate validate.
        /// </summary>
        private static bool HttpsCertificateValidation(
            object sender,
            X509Certificate cert,
            X509Chain chain,
            System.Net.Security.SslPolicyErrors error)
        {
            try
            {
                m_validator.Validate(new X509Certificate2(cert.GetRawCertData()));
                return true;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not verify SSL certificate: {0}", cert.Subject);
                return false;
            }
        }
        #endregion

        #region Tmp certificates
        /// <summary>
        /// Creates the application instance certificate.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="keySize">Size of the key.</param>
        /// <param name="lifetimeInMonths">The lifetime in months.</param>
        /// <returns>The new certificate</returns>
        private static X509Certificate2 CreateApplicationInstanceCertificate(
            ApplicationConfiguration configuration,
            ushort keySize,
            ushort lifetimeInMonths)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Creating application instance certificate. KeySize={0}, Lifetime={1}", keySize, lifetimeInMonths);

            // delete existing any existing certificate.
            DeleteApplicationInstanceCertificate(configuration);

            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            // get the domains from the configuration file.
            IList<string> serverDomainNames = configuration.GetServerDomainNames();

            if (serverDomainNames.Count == 0)
            {
                serverDomainNames.Add(System.Net.Dns.GetHostName());
            }

            // ensure the certificate store directory exists.
            if (id.StoreType == CertificateStoreType.Directory)
            {
                Utils.GetAbsoluteDirectoryPath(id.StorePath, true, true, true);
            }

            X509Certificate2 certificate = Opc.Ua.CertificateFactory.CreateCertificate(
                id.StoreType,
                id.StorePath,
                configuration.ApplicationUri,
                configuration.ApplicationName,
                null,
                serverDomainNames,
                keySize,
                lifetimeInMonths);

            id.Certificate = certificate;
            AddToTrustedStore(configuration, certificate);

            

            configuration.CertificateValidator.Update(configuration.SecurityConfiguration);

            Utils.Trace(Utils.TraceMasks.Information, "Certificate created. Thumbprint={0}", certificate.Thumbprint);

            // reload the certificate from disk.
            return configuration.SecurityConfiguration.ApplicationCertificate.LoadPrivateKey(null);
        }

        /// <summary>
        /// Adds the certificate to the Trusted Certificate Store
        /// </summary>
        /// <param name="configuration">The application's configuration which specifies the location of the TrustedStore.</param>
        /// <param name="certificate">The certificate to register.</param>
        private static void AddToTrustedStore(ApplicationConfiguration configuration, X509Certificate2 certificate)
        {
            string storePath = null;

            if (configuration != null && configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                storePath = configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath;
            }

            if (String.IsNullOrEmpty(storePath))
            {
                Utils.Trace(Utils.TraceMasks.Information, "WARNING: Trusted peer store not specified.");
                return;
            }

            try
            {
                ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore();

                if (store == null)
                {
                    Utils.Trace("Could not open trusted peer store. StorePath={0}", storePath);
                    return;
                }

                try
                {
                    // check if it already exists.
                    X509Certificate2 certificate2 = store.FindByThumbprint(certificate.Thumbprint);

                    if (certificate2 != null)
                    {
                        return;
                    }

                    Utils.Trace(Utils.TraceMasks.Information, "Adding certificate to trusted peer store. StorePath={0}", storePath);

                    List<string> subjectName = Utils.ParseDistinguishedName(certificate.Subject);

                    // check for old certificate.
                    X509Certificate2Collection certificates = store.Enumerate();

                    for (int ii = 0; ii < certificates.Count; ii++)
                    {
                        if (Utils.CompareDistinguishedName(certificates[ii], subjectName))
                        {
                            if (certificates[ii].Thumbprint == certificate.Thumbprint)
                            {
                                return;
                            }

                            store.Delete(certificates[ii].Thumbprint);
                            break;
                        }
                    }

                    // add new certificate.
                    X509Certificate2 publicKey = new X509Certificate2(certificate.GetRawCertData());
                    store.Add(publicKey);
                }
                finally
                {
                    store.Close();
                }
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not add certificate to trusted peer store. StorePath={0}", storePath);
            }
        }

        /// <summary>
        /// Deletes an existing application instance certificate.
        /// </summary>
        /// <param name="configuration">The configuration instance that stores the configurable information for a UA application.</param>
        private static void DeleteApplicationInstanceCertificate(ApplicationConfiguration configuration)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Deleting application instance certificate.");

            // create a default certificate id none specified.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                return;
            }

            // delete private key.
            X509Certificate2 certificate = id.Find();

            // delete trusted peer certificate.
            if (configuration.SecurityConfiguration != null && configuration.SecurityConfiguration.TrustedPeerCertificates != null)
            {
                string thumbprint = id.Thumbprint;

                if (certificate != null)
                {
                    thumbprint = certificate.Thumbprint;
                }

                using (ICertificateStore store = configuration.SecurityConfiguration.TrustedPeerCertificates.OpenStore())
                {
                    store.Delete(thumbprint);
                }
            }

            // delete private key.
            if (certificate != null)
            {
                using (ICertificateStore store = id.OpenStore())
                {
                    store.Delete(certificate.Thumbprint);
                }
            }
        }

        /// <summary>
        /// Adds an application certificate to a store.
        /// </summary>
        private static void AddApplicationCertificateToStore(
            CertificateStoreIdentifier csid,
            X509Certificate2 certificate,
            string oldThumbprint)
        {
            ICertificateStore store = csid.OpenStore();

            try
            {
                // delete the old certificate.
                if (oldThumbprint != null)
                {
                    store.Delete(oldThumbprint);
                }

                // delete certificates with the same application uri.
                if (store.FindByThumbprint(certificate.Thumbprint) == null)
                {
                    string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

                    // delete any existing certificates.
                    foreach (X509Certificate2 target in store.Enumerate())
                    {
                        if (Utils.CompareDistinguishedName(target.Subject, certificate.Subject))
                        {
                            if (Utils.GetApplicationUriFromCertficate(target) == applicationUri)
                            {
                                store.Delete(target.Thumbprint);
                            }
                        }
                    }

                    // add new certificate.
                    store.Add(new X509Certificate2(certificate.RawData));
                }
            }
            finally
            {
                store.Close();
            }
        }

        /// <summary>
        /// Checks for a valid application instance certificate.
        /// </summary>
        /// <param name="silent">if set to <c>true</c> no dialogs will be displayed.</param>
        /// <param name="minimumKeySize">Minimum size of the key.</param>
        public void CheckApplicationInstanceCertificate(
            bool silent,
            ushort minimumKeySize)
        {
            Utils.Trace(Utils.TraceMasks.Information, "Checking application instance certificate.");

            ApplicationConfiguration configuration = null;

            configuration = _applicationConfig;
            bool createNewCertificate = true;

            // find the existing certificate.
            CertificateIdentifier id = configuration.SecurityConfiguration.ApplicationCertificate;

            if (id == null)
            {
                throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Configuration file does not specify a certificate.");
            }

            X509Certificate2 certificate = id.Find(true);

            // check that it is ok.
            if (certificate != null)
            {
                createNewCertificate = !CheckApplicationInstanceCertificate(configuration, certificate, silent, minimumKeySize);
            }
            else
            {
                // check for missing private key.
                certificate = id.Find(false);

                if (certificate != null)
                {
                    throw ServiceResultException.Create(StatusCodes.BadConfigurationError, "Cannot access certificate private key. Subject={0}", certificate.Subject);
                }

                // check for missing thumbprint.
                if (!String.IsNullOrEmpty(id.Thumbprint))
                {
                    if (!String.IsNullOrEmpty(id.SubjectName))
                    {
                        CertificateIdentifier id2 = new CertificateIdentifier();
                        id2.StoreType = id.StoreType;
                        id2.StorePath = id.StorePath;
                        id2.SubjectName = id.SubjectName;

                        certificate = id2.Find(true);
                    }

                    if (certificate != null)
                    {
                        string message = Utils.Format(
                            "Thumbprint was explicitly specified in the configuration." +
                            "\r\nAnother certificate with the same subject name was found." +
                            "\r\nUse it instead?\r\n" +
                            "\r\nRequested: {0}" +
                            "\r\nFound: {1}",
                            id.SubjectName,
                            certificate.Subject);

                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, message);
                    }
                    else
                    {
                        string message = Utils.Format("Thumbprint was explicitly specified in the configuration. Cannot generate a new certificate.");
                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, message);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(id.SubjectName))
                    {
                        string message = Utils.Format("Both SubjectName and Thumbprint are not specified in the configuration. Cannot generate a new certificate.");
                        throw ServiceResultException.Create(StatusCodes.BadConfigurationError, message);
                    }
                }
            }

            // create a new certificate.
            if (createNewCertificate)
            {
                certificate = CreateApplicationInstanceCertificate(configuration, minimumKeySize, 600);
            }

            // ensure it is trusted.
            else
            {
                AddToTrustedStore(configuration, certificate);
            }  
        }

        /// <summary>
        /// Creates an application instance certificate if one does not already exist.
        /// </summary>
        private static bool CheckApplicationInstanceCertificate(
            ApplicationConfiguration configuration,
            X509Certificate2 certificate,
            bool silent,
            ushort minimumKeySize)
        {
            if (certificate == null)
            {
                return false;
            }

            Utils.Trace(Utils.TraceMasks.Information, "Checking application instance certificate. {0}", certificate.Subject);

            // validate certificate.
            configuration.CertificateValidator.Validate(certificate);

            // check key size.
            if (minimumKeySize > certificate.PublicKey.Key.KeySize)
            {
                bool valid = false;

                string message = Utils.Format(
                    "The key size ({0}) in the certificate is less than the minimum provided ({1}). Update certificate?",
                    certificate.PublicKey.Key.KeySize,
                    minimumKeySize);

                Utils.Trace(message);

                if (!valid)
                {
                    return false;
                }
            }

            //// check domains.
            //if (configuration.ApplicationType != ApplicationType.Client)
            //{
            //    if (!CheckDomainsInCertificate(configuration, certificate, silent))
            //    {
            //        return false;
            //    }
            //}

            // update uri.
            string applicationUri = Utils.GetApplicationUriFromCertficate(certificate);

            if (String.IsNullOrEmpty(applicationUri))
            {
                bool valid = false;

                string message = "The Application URI is not specified in the certificate. Update certificate?";

                Utils.Trace(message);

                if (!valid)
                {
                    return false;
                }
            }

            // update configuration.
            configuration.ApplicationUri = applicationUri;
            configuration.SecurityConfiguration.ApplicationCertificate.Certificate = certificate;

            return true;
        }
        #endregion

    }
}