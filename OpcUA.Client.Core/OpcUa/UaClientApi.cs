﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private ApplicationConfiguration _applicationConfig;
        private Session _session;

        #endregion

        #region Constructors

        public UaClientApi()
        {
            // Creats the application configuration (containing the certificate) on construction
            _applicationConfig = CreateClientConfiguration();
            CertificateUtils.CheckApplicationInstanceCertificate(_applicationConfig, false, 2048);
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
                // TODO handle Exception here
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
                ////Create a DiscoveryClient
                using (var discoveryClient = DiscoveryClient.Create(uri))
                {
                    //Search for available endpoints
                    var endpoints = discoveryClient.GetEndpoints(null);
                    return endpoints;
                }
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        #endregion

        #region Public Methods

        public void CreateDefaultConfiguration()
        {
            if (_applicationConfig != null) return;
            _applicationConfig = CreateClientConfiguration();
        }

        public void SaveConfiguration()
        {
            _applicationConfig?.SaveToFile(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents\\UaClient\\Configuration.xml"));
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
                _session.Browse(null,
                                null,
                                ObjectIds.RootFolder,
                                0u,
                                BrowseDirection.Forward,
                                ReferenceTypeIds.HierarchicalReferences,
                                true,
                                (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                                out var continuationPoint,
                                out var referenceDescriptionCollection);

                return referenceDescriptionCollection;
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }

        }

        /// <summary>Browses a node ID provided by a ReferenceDescription</summary>
        /// <param name="referenceDescription">The ReferenceDescription</param>
        /// <returns>ReferenceDescriptionCollection of found nodes</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ReferenceDescriptionCollection BrowseNode(ReferenceDescription referenceDescription)
        {
            var nodeId = ExpandedNodeId.ToNodeId(referenceDescription.NodeId, null);
            try
            {
                //Browse from starting point for all object types
                _session.Browse(null, 
                                null, 
                                nodeId, 
                                0u, 
                                BrowseDirection.Forward,
                                ReferenceTypeIds.HierarchicalReferences, 
                                true, 
                                0,
                                out var continuationPoint,
                                out var referenceDescriptionCollection);

                return referenceDescriptionCollection;
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }

        }
        #endregion

        #region Connect / Disconnect
        /// <summary>Establishes the connection to an OPC UA server and creates a session using an EndpointDescription.</summary>
        /// <param name="endpointDescription">The EndpointDescription of the server's endpoint</param>
        /// <param name="userName">The user name</param>
        /// <param name="password">The password</param>
        /// <param name="sessionName">Name of the session</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Connect(EndpointDescription endpointDescription, string userName, string password, string sessionName)
        {
            try
            {
                var endpointConfiguration = EndpointConfiguration.Create(_applicationConfig);
                var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);


                _applicationConfig.CertificateValidator.CertificateValidation += CertificateValidation;
                _applicationConfig.CertificateValidator.Update(_applicationConfig);

                var userIdentity = new UserIdentity(userName, password);

                _session = Session.Create(
                    _applicationConfig,
                    endpoint,
                    true,
                    sessionName,
                    60000,
                    userIdentity,
                    null
                    );
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        /// <summary>Establishes the connection to an OPC UA server and creates a session using an EndpointDescription.</summary>
        /// <param name="endpointDescription">The EndpointDescription of the server's endpoint</param>
        /// <param name="sessionName"></param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void ConnectAnonymous(EndpointDescription endpointDescription, string sessionName)
        {
            try
            {
                var endpointConfiguration = EndpointConfiguration.Create(_applicationConfig);
                var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                _applicationConfig.CertificateValidator.CertificateValidation += CertificateValidation;
                _applicationConfig.CertificateValidator.Update(_applicationConfig);

                _session = Session.Create(
                    _applicationConfig,
                    endpoint,
                    true,
                    sessionName,
                    60000,
                    null,
                    null
                    );
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        /// <summary>Closes an existing session and disconnects from the server.</summary>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Disconnect()
        {
            if (_session == null) return;
            try
            {
                // TODO unsubscribe subscription and unregister nodes
                RemoveAllSubscriptions();
                _session.Close(10000);
                _session.Dispose();
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        } 

        #endregion

        #region Subscription

        /// <summary>Creats a Subscription object to a server</summary>
        /// <param name="publishingInterval">The publishing interval</param>
        /// <returns>Subscription</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Subscription Subscribe(int publishingInterval)
        {
            //Create a Subscription object
            var subscription = new Subscription(_session.DefaultSubscription)
            {
                PublishingEnabled = true,
                PublishingInterval = publishingInterval,
                TimestampsToReturn = TimestampsToReturn.Both
            };

            _session.AddSubscription(subscription);

            try
            {
                //Create/Activate the subscription
                subscription.Create();

                return subscription;
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        /// <summary>Creats a Subscription object to a server</summary>
        /// <returns>Subscription</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Subscription CreateSubscriptionFromTemplate(Subscription template)
        {
            //Create a Subscription object
            var subscription = new Subscription(template);

            _session.AddSubscription(subscription);

            try
            {
                subscription.Create();
                return subscription;
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        /// <summary>Ads a monitored item to an existing subscription</summary>
        /// <param name="node"></param>
        /// <param name="subscription"></param>
        /// <returns>The added item</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public MonitoredItem AddMonitoredItem(ReferenceDescription node, Subscription subscription)
        {
            var monitoredItem = new MonitoredItem
            {
                DisplayName = node.DisplayName.ToString(),
                StartNodeId = ExpandedNodeId.ToNodeId(node.NodeId, null),
                AttributeId = Attributes.Value,
                MonitoringMode = MonitoringMode.Reporting,
                // -1 minimum value, 1 maximum value
                SamplingInterval = 1,
                QueueSize = 1,
                DiscardOldest = true,
            };
            try
            {
                subscription.AddItem(monitoredItem);
                subscription.ApplyChanges();

                return monitoredItem;
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        /// <summary>Removs a monitored item from an existing subscription</summary>
        /// <param name="subscription">The subscription</param>
        /// <param name="monitoredItem">The item</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void RemoveMonitoredItem(Subscription subscription, MonitoredItem monitoredItem)
        {
            try
            {
                subscription.RemoveItem(monitoredItem);
                subscription.ApplyChanges();
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        /// <summary>Removes an existing Subscription.</summary>
        /// <param name="subscription">The subscription</param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void RemoveSubscription(Subscription subscription)
        {
            try
            {
                subscription.RemoveItems(subscription.MonitoredItems);
                
                subscription.Delete(true);
                subscription.Dispose();
                
                subscription.ApplyChanges();

                _session.RemoveSubscription(subscription);
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        /// <summary>Removes an existing Subscription.</summary>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        private void RemoveAllSubscriptions()
        {
            try
            {
                foreach (var subscription in _session.Subscriptions)
                    RemoveSubscription(subscription);
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                throw e;
            }
        }

        public void SaveSubsciption()
        {
            _session.Save(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents\\UaClient\\Subscriptions.xml"));
        }

        public Subscription LoadSubsciption()
        {
            var subscription = _session.Load(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents\\UaClient\\Subscriptions.xml")).FirstOrDefault();
            return CreateSubscriptionFromTemplate(subscription);
        }

        public List<Subscription> LoadSubsciptions()
        {
            return _session.Load(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents\\UaClient\\Subscriptions.xml")).ToList();
        }

        #endregion

        #region Read/Write

        /// <summary>
        /// Reads a node by node Id from server address space
        /// </summary>
        /// <param name="nodeIdString"></param>
        /// <returns>The read node</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Node ReadNode(string nodeIdString)
        {
            var nodeId = new NodeId(nodeIdString);
            try
            {
                //Read the dataValue
                var node = _session.ReadNode(nodeId);
                return node;
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                Utils.Trace(Utils.TraceMasks.Error, "Error");

                throw e;
            }
        }

        /// <summary>
        /// Reads a node by node Id from server address space
        /// </summary>
        /// <param name="expandedNodeId"></param>
        /// <returns>The read node</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public Node ReadNode(ExpandedNodeId expandedNodeId)
        {
            var nodeId = ExpandedNodeId.ToNodeId(expandedNodeId, null);
            try
            {
                //Read the dataValue
                var node = _session.ReadNode(nodeId);
                return node;
            }
            catch (Exception e)
            {
                // TODO handle Exception here
                Utils.Trace(Utils.TraceMasks.Error, "Error");

                throw e;
            }
        }

        /// <summary>
        /// Reads Node of <see cref="nodeId"/> dataType in server address space.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public Node GetDataTypeOfVariableNodeId(NodeId nodeId)
        {
            var node = _session.ReadNode(nodeId);
            var dataTypeNodeId = (node.DataLock as VariableNode)?.DataType;
            var dataTypeNode = _session.ReadNode(dataTypeNodeId);
            return dataTypeNode;
        }

        public bool WriteValue(Variable variable, object newValue)
        {
            var wrapedValue = new Variant(Convert.ChangeType(newValue, variable.DataType));
            var data = new DataValue(wrapedValue);

            var valueToWrite = new WriteValue()
            {
                Value = data,
                NodeId = variable.MonitoredItem.StartNodeId,
                AttributeId = Attributes.Value,   
            };

            try
            {
                _session.Write(null, new WriteValueCollection() { valueToWrite }, out var statusCodes, out var diagnosticInfo);

                if (statusCodes.FirstOrDefault().Code == StatusCodes.Good) { 
                    Utils.Trace(Utils.TraceMasks.Information, "Value successfully written to server!");
                    return true;
                }

                Utils.Trace(Utils.TraceMasks.Error, "Value was not written to server!");
                return false;
            }
            catch (Exception e)
            {
                // TODO handle exception
                throw e;
            }
        }

        public DataValue ReadValue(NodeId nodeId)
        {
            return _session.ReadValue(nodeId);
        }

        #endregion

            #region Private methods

            /// <summary>Creates a default application configuration.</summary>
            /// <returns>The ApplicationConfiguration</returns>
            /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
            private ApplicationConfiguration CreateClientConfiguration()
        {
            var configuration = new ApplicationConfiguration
            {
                ApplicationName = "UaClient",
                ApplicationType = ApplicationType.Client,
                ApplicationUri = "urn:UaClient",
                ProductUri = "VutBrno.SchoolProject",

                ClientConfiguration = new ClientConfiguration
                {
                    DefaultSessionTimeout = 360000,
                    // DiscoveryServers = 
                    // EndpointCacheFilePath = 
                    // MinSubscriptionLifetime = 
                    // WellKnownDiscoveryUrls = 
                },

                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = 360000,
                    MaxStringLength = 67108864,
                    MaxByteStringLength = 16777216,
                    // ChannelLifetime =
                    // MaxArrayLength = 
                    // MaxBufferSize = 
                    // MaxMessageSize = 
                    // SecurityTokenLifetime = 
                },

            };
            configuration.ClientConfiguration.Validate();

            // Add trace config before calling validate
            configuration.TraceConfiguration = new TraceConfiguration
            {
                OutputFilePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"),
                    "Documents\\UaClient\\TraceLogs.txt"),
                DeleteOnLoad = true,
                TraceMasks = Utils.TraceMasks.All,
            };
            configuration.TraceConfiguration.ApplySettings();

            Utils.SetTraceLog(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"),
                "Documents\\UaClient\\UtilsTraceLogs.txt"), true);
            Utils.SetTraceMask(Utils.TraceMasks.All);
            Utils.SetTraceOutput(Utils.TraceOutput.DebugAndFile);


            configuration.SecurityConfiguration = new SecurityConfiguration
            {
                AutoAcceptUntrustedCertificates = true,

                RejectedCertificateStore = new CertificateStoreIdentifier()
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "CurrentUser\\Rejected"
                },
                TrustedIssuerCertificates =
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "CurrentUser\\Root",
                },
                TrustedPeerCertificates =
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "CurrentUser\\Root",
                },
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = CertificateStoreType.Windows,
                    StorePath = "CurrentUser\\My",
                    SubjectName = configuration.ApplicationName,
                }
            };

            configuration.SecurityConfiguration.Validate();

            configuration.CertificateValidator = new CertificateValidator();
            configuration.CertificateValidator.Update(configuration.SecurityConfiguration);
            // Set certificate validation
            //configuration.CertificateValidator.CertificateValidation += CertificateValidation;
            // Set Https certificate validation
            ServicePointManager.ServerCertificateValidationCallback = HttpsCertificateValidation;

            configuration.Validate(ApplicationType.Client);

            return configuration;
        }

        private void ConvertToSpecificType(object value, string type)
        {
            //switch (type)
            //{
            //      case  
            //}
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Eventhandler to validate the server certificate forwards this event
        /// </summary>
        private static void CertificateValidation(CertificateValidator certificate, CertificateValidationEventArgs e)
        {
            try
            {
                if (e.Error == null || e.Error.Code != StatusCodes.BadCertificateUntrusted) return;
                e.Accept = true;
                Utils.Trace(Utils.TraceMasks.Security, "Automatically accepted certificate: {0}", e.Certificate.Subject);

                // Always save to store
                X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                if (!store.Certificates.Contains(e.Certificate))
                    store.Add(e.Certificate);

                Utils.Trace(Utils.TraceMasks.Security, "Certificate added to trusted store: {0}", e.Certificate.Subject);
            }
            catch (Exception exception)
            {
                Utils.Trace(exception, "Error accepting certificate.");
            }  
        }

        /// <summary>
        /// HTTPS certificates validation.
        /// </summary>
        private bool HttpsCertificateValidation(
            object sender,
            X509Certificate cert,
            X509Chain chain,
            System.Net.Security.SslPolicyErrors error)
        {
            try
            {
                _applicationConfig.CertificateValidator.Validate(new X509Certificate2(cert.GetRawCertData()));
                return true;
            }
            catch (Exception e)
            {
                Utils.Trace(e, "Could not verify SSL certificate: {0}", cert.Subject);
                return false;
            }
        }

        #endregion
    }
}