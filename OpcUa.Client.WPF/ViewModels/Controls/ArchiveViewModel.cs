using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Opc.Ua;
using Opc.Ua.Client;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class ArchiveViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly UaClientApi _uaClientApi;
        private readonly Messenger _messenger;
        private Subscription _subscription;

        private ReferenceDescription _selectedNode;
        private readonly Dictionary<ArchiveInterval, Timer> _timers = new Dictionary<ArchiveInterval, Timer>();
        private List<ArchiveReadVariableModel> _registeredNodesForRead;
        #endregion

        #region Public Properties

        public ObservableCollection<VariableModel> ArchiveVariables { get; set; }
        public VariableModel SelectedArchiveVariable { get; set; }
       
        public ObservableCollection<ArchiveListModel> ArchiveInfo { get; set; }
        public ArchiveListModel SelectedArchiveInfo { get; set; }
        #endregion

        #region Commands
        public ICommand AddVariableToArchiveCommand { get; }
        public ICommand DeleteVariableFromArchiveCommand { get; }
        public ICommand StartArchiveCommand { get; }
        public ICommand StopArchiveCommand { get; }
        #endregion

        #region Constructor
        public ArchiveViewModel(IUnitOfWork unitOfWork, UaClientApi uaClientApi, Messenger messenger)
        {
            _unitOfWork = unitOfWork;
            _uaClientApi = uaClientApi;
            _messenger = messenger;

            OnLoad();
           
            AddVariableToArchiveCommand = new MixRelayCommand(AddVariableToArchive, AddVariableCanUse);
            DeleteVariableFromArchiveCommand = new MixRelayCommand(DeleteVariableFromArchive, DeleteVariableCanUse);
            StartArchiveCommand = new MixRelayCommand(StartArchive, StartArchiveCanUse);
            StopArchiveCommand = new MixRelayCommand(StopArchive, StopArchiveCanUse);

            _messenger.Register<SendSelectedRefNode>(msg => _selectedNode = msg.ReferenceNode);
        }
        #endregion

        #region Command Methods
        private void StartArchive(object parameter)
        {
            var interval = SelectedArchiveInfo.ArchiveInterval;

            if (interval == ArchiveInterval.None)
            {
                //_subscription.PublishingEnabled = true;
                _subscription.SetMonitoringMode(MonitoringMode.Reporting, _subscription.MonitoredItems.ToList());
                _subscription.ApplyChanges();
            }
            else
            {
                var timer = new Timer(Archive, interval, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds((int)interval));
                _timers.Add(interval, timer);
            }
          
            SelectedArchiveInfo.Running = true;
        }

        private void StopArchive(object parameter)
        {
            var interval = SelectedArchiveInfo.ArchiveInterval;
            if (interval == ArchiveInterval.None)
            {
                //_subscription.PublishingEnabled = false;
                _subscription.SetMonitoringMode(MonitoringMode.Disabled, _subscription.MonitoredItems.ToList());
                _subscription.ApplyChanges();
            }
            else
            {
                _timers.TryGetValue(interval, out var timer);

                if (timer == null) return;
                timer.Dispose();
                _timers.Remove(interval);
            }

            SelectedArchiveInfo.Running = false;
        }

        private void AddVariableToArchive(object parameter)
        {
            var nodeId = _selectedNode.NodeId.ToString();
            var type = _uaClientApi.GetBuiltInTypeOfVariableNodeId(nodeId);
            var interval = SelectedArchiveInfo.ArchiveInterval;

            var tmp = new VariableEntity()
            {
                Archive = interval,
                Name = nodeId,
                DataType = type,
                ProjectId = IoC.AppManager.ProjectId
            };
            _unitOfWork.Variables.Add(tmp);

            var variableModel = Mapper.VariableEntityToVariableModel(tmp);
            _messenger.Send(new SendManageArchivedValue(false, variableModel));
            ArchiveVariables.Add(variableModel);

            try
            {
                if (interval != ArchiveInterval.None)
                {
                    var registeredNode = _uaClientApi.RegisterNode(nodeId);
                    _registeredNodesForRead.Add(new ArchiveReadVariableModel()
                    {
                        Interval = interval,
                        RegisteredNodeId = registeredNode,
                        Type = type,
                        VariableId = tmp.Id
                    });
                }
                else
                {
                    var item = _uaClientApi.CreateMonitoredItem($"{nodeId} [{ArchiveInterval.None}]", nodeId, 100, null, 2, MonitoringMode.Disabled);
                    item.Notification += Notification_MonitoredItem;
                    _uaClientApi.AddMonitoredItem(item, _subscription);
                }
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }
           
            SelectedArchiveInfo.VariablesCount++;
        }

        private void DeleteVariableFromArchive(object parameter)
        {
            var interval = SelectedArchiveVariable.Archive;

            foreach (var archive in ArchiveInfo)
                if (archive.ArchiveInterval == interval)
                    archive.VariablesCount--;

            // Vymazanie z databaze
            _unitOfWork.Variables.DeleteById(SelectedArchiveVariable.Id);
            //_unitOfWork.Variables.Remove(Mapper.VariableModelToVariableEntity(SelectedArchiveVariable));
            _messenger.Send(new SendManageArchivedValue(true, SelectedArchiveVariable));

            try
            {
                if (interval == ArchiveInterval.None)
                    _uaClientApi.RemoveMonitoredItem(_subscription, SelectedArchiveVariable.Name);
                else
                {
                    // Odregistrovanie
                    var var = _registeredNodesForRead.FirstOrDefault(x => x.VariableId == SelectedArchiveVariable.Id);
                    _uaClientApi.UnRegisterNode(var?.RegisteredNodeId);
                    // Vymazanie z nodes for read
                    _registeredNodesForRead.Remove(var);
                }
                // Vymazanie z tabulky
                ArchiveVariables.Remove(SelectedArchiveVariable);

            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }
        }
        #endregion

        #region Can use methods
        public bool DeleteVariableCanUse(object parameter)
        {
            ArchiveListModel first = null;
            foreach (var x in ArchiveInfo)
            {
                if (x.ArchiveInterval != SelectedArchiveVariable?.Archive) continue;
                first = x;
                break;
            }

            return first != null && (SelectedArchiveVariable != null && !first.Running);
        }

        public bool AddVariableCanUse(object parameter)
        {
            return _selectedNode != null && SelectedArchiveInfo != null &&
                   _selectedNode.NodeClass == NodeClass.Variable && !SelectedArchiveInfo.Running;
        }

        public bool StartArchiveCanUse(object parameter)
        {
            return SelectedArchiveInfo != null && !SelectedArchiveInfo.Running && SelectedArchiveInfo.VariablesCount != 0;
        }

        public bool StopArchiveCanUse(object parameter)
        {
            return SelectedArchiveInfo != null && SelectedArchiveInfo.Running;
        }
        #endregion

        #region Private Methods
        private void OnLoad()
        {
            _subscription = _uaClientApi.Subscribe(2000, "Archivation");
            if (_subscription == null)
                IoC.AppManager.ShowWarningMessage("Subscription creation failed, please restart application!");

            LoadDataFromDataBase();
            RegisterLoadedNodes();
            InitializeArchiveTable();
        }

        private bool IsTimerAlive(ArchiveInterval interval)
        {
            _timers.TryGetValue(interval, out var timer);
            return timer != null;
        }

        private void LoadDataFromDataBase()
        {
            ArchiveVariables = new ObservableCollection<VariableModel>(
                Mapper.VariableEntitiesToVariableListModels( _unitOfWork.Variables.Find(x => x.ProjectId == IoC.AppManager.ProjectId) ));
        }

        private void RegisterLoadedNodes()
        {
            var nodeIds = ArchiveVariables.Where(x => x.Archive != ArchiveInterval.None).Select(x => x.Name).ToList();
            try
            {
                var registeredNodes = _uaClientApi.RegisterNodes(nodeIds);

                _registeredNodesForRead = ArchiveVariables.Where(x => x.Archive != ArchiveInterval.None)
                                                          .Zip(registeredNodes, (entity, regNode) => new ArchiveReadVariableModel()
                                                          {
                                                              VariableId = entity.Id,
                                                              RegisteredNodeId = regNode,
                                                              Type = entity.DataType,
                                                              Interval = entity.Archive
                                                          }).ToList();
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }

            foreach (var variable in ArchiveVariables.Where(x => x.Archive == ArchiveInterval.None))
            {
                var item = _uaClientApi.CreateMonitoredItem($"{variable.Name} [{ArchiveInterval.None}]", variable.Name, 100, null, 2, MonitoringMode.Disabled);
                item.Notification += Notification_MonitoredItem;
                _uaClientApi.AddMonitoredItem(item, _subscription);
            }
        }

        private void InitializeArchiveTable()
        {
            var archiveIntervals = Enum.GetValues(typeof(ArchiveInterval)).Cast<ArchiveInterval>();
            ArchiveInfo = new ObservableCollection<ArchiveListModel>();

            foreach (var interval in archiveIntervals)
            {
                ArchiveInfo.Add(new ArchiveListModel()
                {
                    ArchiveInterval = interval,
                    VariablesCount = ArchiveVariables.Count(x => x.Archive == interval),
                    Running = false
                });
            }
        }

        private void Archive(object objectInfo)
        {
            var interval = (ArchiveInterval)objectInfo;
            if (!IsTimerAlive(interval)) return;

            // Vytriedenie premennych pre tento interval a ancitanie hodnot
            var variablesForRead = _registeredNodesForRead.Where(x => x.Interval == interval).ToList();
            try
            {
                _uaClientApi.ReadValues(ref variablesForRead);
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                //IoC.AppManager.ShowExceptionErrorMessage(e);
            }

            // Vytvorenie zaznamov
            var records = variablesForRead.Select(x => new RecordEntity()
            {
                VariableId = x.VariableId,
                Value = x.Value.ToString(),
                ArchiveTime = DateTime.Now,
            }).ToList();

            // Archivacia
            _unitOfWork.Records.AddRange(records);
        }
        #endregion

        #region CallBack Methods
        /// <summary>
        /// Callback method for updating values of subscibed nodes
        /// </summary>
        /// <param name="monitoredItem"></param>
        /// <param name="e"></param>
        private void Notification_MonitoredItem(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (!(e.NotificationValue is MonitoredItemNotification notification))
                return;

            var value = notification.Value;

            var variable = ArchiveVariables.FirstOrDefault(x =>
                x.Name == monitoredItem.StartNodeId && monitoredItem.DisplayName == $"{x.Name} [{ArchiveInterval.None}]");

            if (variable == null) return;

            _unitOfWork.Records.Add(new RecordEntity()
            {
                ArchiveTime = value.SourceTimestamp,
                VariableId = variable.Id,
                Value = value.Value.ToString()
            });
        }
        #endregion
    }
}