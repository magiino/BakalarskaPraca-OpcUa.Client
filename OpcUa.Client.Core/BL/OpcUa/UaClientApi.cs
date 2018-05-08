using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Ninject.Infrastructure.Language;
using Opc.Ua;
using Opc.Ua.Client;

namespace OpcUa.Client.Core
{
    public class UaClientApi
    {
        #region Private Fields
        private readonly ApplicationConfiguration _applicationConfig;
        private Session _session;
        private readonly NodeIdCollection _registeredNodes = new NodeIdCollection();
        #endregion

        #region Public Fields
        public bool SessionState { get; set; }
        #endregion

        #region Constructors
        public UaClientApi()
        {
            _applicationConfig = CreateClientConfiguration();
            CertificateUtils.CheckApplicationInstanceCertificate(_applicationConfig, false, 2048);
        }
        #endregion

        #region Discovery Client
        public ApplicationDescriptionCollection FindServers(string discoveryUrl)
        {
            var uri = new Uri(discoveryUrl);

            using (var client = DiscoveryClient.Create(uri))
            {
                //Find servers
                var servers = client.FindServers(null);
                return servers;
            }
        }

        public EndpointDescriptionCollection GetEndpoints(string serverUrl)
        {
            //Create a URI using the server's URL
            var uri = new Uri(serverUrl);

            ////Create a DiscoveryClient
            using (var discoveryClient = DiscoveryClient.Create(uri))
            {
                //Search for available endpoints
                var endpoints = discoveryClient.GetEndpoints(null);
                return endpoints;
            }
        }
        #endregion

        #region Browse
        public ReferenceDescriptionCollection BrowseRoot()
        {
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

        /// <summary>Browses a node ID provided by a ReferenceDescription</summary>
        /// <param name="referenceDescription">The ReferenceDescription</param>
        /// <returns>ReferenceDescriptionCollection of found nodes</returns>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public ReferenceDescriptionCollection BrowseNode(ReferenceDescription referenceDescription)
        {
            var expNodeId = referenceDescription.NodeId;
            var nodeId = ExpandedNodeId.ToNodeId(expNodeId, new NamespaceTable());

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

        #endregion

        #region Connect / Disconnect
        public bool Connect(EndpointDescription endpointDescription, string userName, string password, string sessionName)
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
            _session.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);
            return _session != null;
        }

        /// <summary>Establishes the connection to an OPC UA server and creates a session using an EndpointDescription.</summary>
        /// <param name="endpointDescription">The EndpointDescription of the server's endpoint</param>
        /// <param name="sessionName"></param>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void ConnectAnonymous(EndpointDescription endpointDescription, string sessionName)
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

            _session.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);
        }

        /// <summary>Closes an existing session and disconnects from the server.</summary>
        /// <exception cref="Exception">Throws and forwards any exception with short error description.</exception>
        public void Disconnect()
        {
            if (_session == null) return;
            try
            {
                UnRegisterNodes();
                _session.Close(5000);
                _session.Dispose();
                SessionState = false;
                _session = null;
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
            }
        } 

        #endregion

        #region Subscription / Monitored Item
        public Subscription Subscribe(int publishingInterval, string displayName, bool enablePublishing = true)
        {
            var subscription = new Subscription(_session.DefaultSubscription)
            {
                PublishingEnabled = enablePublishing,
                PublishingInterval = publishingInterval,
                TimestampsToReturn = TimestampsToReturn.Both,
                DisplayName = displayName,
            };
            _session.AddSubscription(subscription);

            try
            {
                subscription.Create();
                return subscription;
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                return null;
            }
        }

        public bool AddMonitoredItem(MonitoredItem monitoredItem, Subscription subscription)
        {
            try
            {
                subscription.AddItem(monitoredItem);
                subscription.ApplyChanges();
                return true;
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
            }

            return false;
        }

        public MonitoredItem CreateMonitoredItem(string displayName, string nodeId, int samplingInterval, MonitoringFilter filterValue = null, int queueSize = 1, MonitoringMode mode = MonitoringMode.Reporting)
        {
            var monitoredItem = new MonitoredItem
            {
                DisplayName = displayName,
                StartNodeId = new NodeId(nodeId),
                AttributeId = Attributes.Value,
                MonitoringMode = mode,
                SamplingInterval = samplingInterval,
                QueueSize = (uint)queueSize,
                CacheQueueSize = queueSize,
                DiscardOldest = true
            };

            if (filterValue == null) return monitoredItem;

            monitoredItem.Filter = filterValue;
            return monitoredItem;
        }

        public void RemoveMonitoredItem(Subscription subscription, string monitoredItemNodeId)
        {
            var tmp = subscription.MonitoredItems.FirstOrDefault(x => x.StartNodeId.ToString() == monitoredItemNodeId);
            subscription.RemoveItem(tmp);
            subscription.ApplyChanges();
        }
        #endregion

        #region Read / Write
        public Node ReadNode(ExpandedNodeId expandedNodeId)
        {
            var nodeId = ExpandedNodeId.ToNodeId(expandedNodeId, new NamespaceTable());

            var node = _session.ReadNode(nodeId);
            return node;
        }

        public DataValue WriteValue(NodeId ndoeId, BuiltInType datayType, object newValue)
        {
            var wrapedValue = new Variant( Convert.ChangeType(newValue, TypeInfo.GetSystemType(datayType, -1)) );
            var data = new DataValue(wrapedValue);

            var valueToWrite = new WriteValue()
            {
                Value = data,
                NodeId = ndoeId,
                AttributeId = Attributes.Value,
            };

            try
            {
                _session.Write(null, new WriteValueCollection() { valueToWrite }, out var statusCodes, out var diagnosticInfo);

                if (statusCodes.FirstOrDefault().Code == StatusCodes.Good) { 
                    Utils.Trace(Utils.TraceMasks.Information, "Value successfully written to server!");
                    data.SourceTimestamp= DateTime.Now;
                    return data;
                }

                Utils.Trace(Utils.TraceMasks.Error, "Value was not written to server!");
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
            }
            return null;
        }

        public DataValue ReadValue(NodeId nodeId)
        {
            return _session.ReadValue(nodeId);
        }

        public void ReadValues(ref List<ArchiveReadVariableModel> variables)
        {
            var dataTypes = variables.Select(x => TypeInfo.GetSystemType(x.Type, -1)).ToList();
            var nodeIds = variables.Select(x => x.RegisteredNodeId).ToList();
            _session.ReadValues(nodeIds, dataTypes, out var values, out var results);

            for (var i = 0; i < values.Count; ++i)
            {
                variables[i].Value = values[i];
                variables[i].Result = results[i];
            }
        }
        #endregion

        #region Register / Unregister
        public List<NodeId> RegisterNodes(List<string> nodesToRegister)
        {
            if (nodesToRegister.Count == 0) return new List<NodeId>();
            var nodeIdsToRegister = new NodeIdCollection(nodesToRegister.Select(x => new NodeId(x)).ToEnumerable());


            _session.RegisterNodes(null, nodeIdsToRegister, out var registeredNodeIds);
            _registeredNodes.AddRange(registeredNodeIds);
            return registeredNodeIds;
        }

        public NodeId RegisterNode(string nodeToRegister)
        {
            var nodeIdToRegister = new NodeIdCollection()
            {
                new NodeId(nodeToRegister)
            };

            _session.RegisterNodes(null, nodeIdToRegister, out var registeredNode);
            var node = registeredNode.FirstOrDefault();
            _registeredNodes.Add(node);
            return node;
        }

        public void UnRegisterNodes()
        {
            try
            {
                _session.UnregisterNodes(null, _registeredNodes);
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
            }
        }

        public bool UnRegisterNode(NodeId nodeIdForUnregister)
        {
            try
            {
                var response = _session.UnregisterNodes(null, new NodeIdCollection(){ nodeIdForUnregister});

                if (!ServiceResult.IsGood(response.ServiceResult)) return false;
                _registeredNodes.Remove(nodeIdForUnregister);
                return true;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region Helper Methods
        public Node GetDataTypeOfVariableNodeId(NodeId nodeId)
        {
            var node = _session.ReadNode(nodeId);
            var dataTypeNodeId = (node.DataLock as VariableNode)?.DataType;
            var dataTypeNode = _session.ReadNode(dataTypeNodeId);
            return dataTypeNode;
        }

        public BuiltInType GetBuiltInTypeOfVariableNodeId(string nodeId)
        {
            var node = _session.ReadNode(new NodeId(nodeId));
            var dataTypeNodeId = (node.DataLock as VariableNode)?.DataType;
            var dataTypeNode = _session.ReadNode(dataTypeNodeId);
            return TypeInfo.GetBuiltInType(dataTypeNode.NodeId);
        }
        #endregion

        #region Private methods
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


            // Set up for Trace
            //Add trace config before calling validate
            //configuration.TraceConfiguration = new TraceConfiguration
            //{
            //    OutputFilePath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"),
            //        "Documents\\UaClient\\TraceLogs.txt"),
            //    DeleteOnLoad = true,
            //    TraceMasks = 0x40,
            //};
            //configuration.TraceConfiguration.ApplySettings();

            //Utils.SetTraceLog(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"),
            //    "Documents\\UaClient\\UtilsTraceLogs.txt"), true);
            //Utils.SetTraceMask(Utils.TraceMasks.All);
            //Utils.SetTraceOutput(Utils.TraceOutput.DebugAndFile);

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
            configuration.CertificateValidator.CertificateValidation += CertificateValidation;
            // Set Https certificate validation
            ServicePointManager.ServerCertificateValidationCallback = HttpsCertificateValidation;

            configuration.Validate(ApplicationType.Client);

            return configuration;
        }
        #endregion

        #region Event Handling
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

        private void Notification_KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            try
            {
                SessionState = true;
                if (!Object.ReferenceEquals(sender, _session))
                    return;

                if (ServiceResult.IsBad(e.Status))
                {
                    SessionState = false;
                    _session.Reconnect();
                }
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "Error accepting certificate.");
            }
        }
        #endregion
    }
}