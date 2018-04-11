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
        private readonly Subscription _subscription;

        private ReferenceDescription _selectedNode;
        private readonly Dictionary<ArchiveInterval, Timer> _timers = new Dictionary<ArchiveInterval, Timer>();
        private List<ArchiveReadVariableModel> _registeredNodesForRead;

        #endregion

        #region Public Properties

        public ObservableCollection<VariableEntity> ArchiveVariables { get; set; }
        public VariableEntity SelectedArchiveVariable { get; set; }
       
        public ObservableCollection<ArchiveListModel> ArchiveInfo { get; set; }
        public ArchiveListModel SelectedArchiveInfo { get; set; }
        #endregion

        #region Commands
        public ICommand AddVariableToArchiveCommand { get; set; }
        public ICommand DeleteVariableFromArchiveCommand { get; set; }
        public ICommand StartArchiveCommand { get; set; }
        public ICommand StopArchiveCommand { get; set; }
        #endregion

        #region Constructor

        public ArchiveViewModel(IUnitOfWork unitOfWork, UaClientApi uaClientApi, Messenger messenger)
        {
            _unitOfWork = unitOfWork;
            _uaClientApi = uaClientApi;
            _messenger = messenger;

            _subscription = _uaClientApi.Subscribe(2000, "Archivation", false);

            LoadDataFromDataBase();
            RegisterLoadedNodes();
            InitializeArchiveTable();

            //AddVariableToArchiveCommand = new RelayCommand(AddVariableToArchive);
            //DeleteVariableFromArchiveCommand = new RelayCommand(DeleteVariableFromArchive);
            //StartArchiveCommand = new RelayCommand(StartArchive);
            //StopArchiveCommand = new RelayCommand(StopArchive);

            AddVariableToArchiveCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(AddVariableToArchive, AddVariableCanUse);
            DeleteVariableFromArchiveCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(DeleteVariableFromArchive, DeleteVariableCanUse);
            StartArchiveCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(StartArchive, StartArchiveCanUse);
            StopArchiveCommand = new GalaSoft.MvvmLight.CommandWpf.RelayCommand(StopArchive, StopArchiveCanUse);

            _messenger.Register<SendSelectedRefNode>(msg => _selectedNode = msg.ReferenceNode);
        }

        #endregion

        #region Command Methods

        private void StartArchive()
        {
            var interval = SelectedArchiveInfo.ArchiveInterval;

            if (interval == ArchiveInterval.None)
            {
                _subscription.PublishingEnabled = true;
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

        private void StopArchive()
        {
            var interval = SelectedArchiveInfo.ArchiveInterval;
            if (interval == ArchiveInterval.None)
            {
                _subscription.PublishingEnabled = false;
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

        private void AddVariableToArchive()
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
            _unitOfWork.Complete();
            _messenger.Send(new SendManageArchivedValue(false, tmp));
            ArchiveVariables.Add(tmp);

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
                var item = _uaClientApi.CreateMonitoredItem(interval.ToString(), nodeId, 500, null, 2, MonitoringMode.Disabled);
                _uaClientApi.AddMonitoredItem(item, _subscription);
                item.Notification += Notification_MonitoredItem;
            }
           
            SelectedArchiveInfo.VariablesCount++;
        }

        private void DeleteVariableFromArchive()
        {
            var interval = SelectedArchiveVariable.Archive;

            foreach (var archive in ArchiveInfo)
                if (archive.ArchiveInterval == interval)
                    archive.VariablesCount--;

            // Vymazanie z databaze
            _unitOfWork.Variables.Remove(SelectedArchiveVariable);
            _messenger.Send(new SendManageArchivedValue(true, SelectedArchiveVariable));

            // Najdenie indexu
            var index = ArchiveVariables.IndexOf(SelectedArchiveVariable);
            // Vymazanie z tabulky
            ArchiveVariables.Remove(SelectedArchiveVariable);
            if (interval == ArchiveInterval.None)
                _uaClientApi.RemoveMonitoredItem(_subscription, SelectedArchiveVariable.Name);
            else
            {
                // Odregistrovanie
                _uaClientApi.UnRegisterNode(_registeredNodesForRead[index].RegisteredNodeId);
                // Vymazanie z nodes for read
                _registeredNodesForRead.RemoveAt(index);
            }
        }
        #endregion

        #region Can use methods

        public bool DeleteVariableCanUse()
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

        public bool AddVariableCanUse()
        {
            return _selectedNode != null && SelectedArchiveInfo != null &&
                   _selectedNode.NodeClass == NodeClass.Variable && !SelectedArchiveInfo.Running;
        }

        public bool StartArchiveCanUse()
        {
            return SelectedArchiveInfo != null && !SelectedArchiveInfo.Running && SelectedArchiveInfo.VariablesCount != 0;
        }

        public bool StopArchiveCanUse()
        {
            return SelectedArchiveInfo != null && SelectedArchiveInfo.Running;
        }

        #endregion

        #region Private Methods

        private void Archive(object objectInfo)
        {
            var interval = (ArchiveInterval)objectInfo;
            if (!IsTimerAlive(interval)) return;

            // Vytriedenie premennych pre tento interval a ancitanie hodnot
            var variablesForRead = _registeredNodesForRead.Where(x => x.Interval == interval).ToList();
            _uaClientApi.ReadValues(ref variablesForRead);

            // Vytvorenie zaznamov
            var records = variablesForRead.Select(x => new RecordEntity()
            {
                VariableId = x.VariableId,
                Value = x.Value.ToString(),
                ArchiveTime = DateTime.Now,
            }).ToList();

            // Archivacia
            _unitOfWork.Records.AddRange(records);
            _unitOfWork.Complete();
        }

        private bool IsTimerAlive(ArchiveInterval interval)
        {
            _timers.TryGetValue(interval, out var timer);
            return timer != null;
        }

        private void LoadDataFromDataBase()
        {
            ArchiveVariables = new ObservableCollection<VariableEntity>(_unitOfWork.Variables.Find(x => x.ProjectId == IoC.AppManager.ProjectId));
        }

        private void RegisterLoadedNodes()
        {
            var nodeIds = ArchiveVariables.Select(x => x.Name).ToList();
            var registeredNodes =_uaClientApi.RegisterNodes(nodeIds);

            _registeredNodesForRead = ArchiveVariables.Where(x => x.Archive != ArchiveInterval.None)
                                                      .Zip(registeredNodes, (entity, regNode) => new ArchiveReadVariableModel()
                                                      {
                                                          VariableId = entity.Id,
                                                          RegisteredNodeId = regNode,
                                                          Type = entity.DataType,
                                                          Interval = entity.Archive
                                                      }).ToList();

            foreach (var variable in ArchiveVariables.Where(x => x.Archive == ArchiveInterval.None))
            {
                var item = _uaClientApi.CreateMonitoredItem(variable.Name, variable.Name, 500, null, 2, MonitoringMode.Disabled);
                _uaClientApi.AddMonitoredItem(item, _subscription);
                item.Notification += Notification_MonitoredItem;
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
                x.Name == monitoredItem.StartNodeId && monitoredItem.DisplayName == ArchiveInterval.None.ToString());

            if (variable == null) return;

            _unitOfWork.Records.Add(new RecordEntity()
            {
                ArchiveTime = value.SourceTimestamp,
                VariableId = variable.Id,
                Value = value.Value.ToString()
            });

            _unitOfWork.Complete();
        }

        #endregion
    }
}
