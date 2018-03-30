using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Opc.Ua;
using OpcUa.Client.Core;

namespace OpcUa.Client.Core
{
    public class ArchiveViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly DataContext _dataContext;
        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _refDescOfSelectedNode;
        private readonly Dictionary<ArchiveInterval, Timer> _timers = new Dictionary<ArchiveInterval, Timer>();
        private List<ArchiveReadVariableModel> _registeredNodesForRead;

        #endregion

        #region Public Properties

        public ObservableCollection<VariableEntity> ArchiveVariables { get; set; }
        public VariableEntity SelectedArchiveVariable { get; set; }
       
        public ObservableCollection<ArchiveListModel> ArchiveInfo { get; set; }
        public ArchiveListModel SelectedArchiveInfo { get; set; }

        public bool ArchiveInfoIsSelected => SelectedArchiveInfo != null;
        public bool AddArchiveVariableIsEnabled { get; set; }
        public bool DeleteArchiveVariableIsEnabled { get; set; }
        #endregion

        #region Commands
        public ICommand AddVariableToArchiveCommand { get; set; }
        public ICommand DeleteVariableFromArchiveCommand { get; set; }
        public ICommand StartArchiveCommand { get; set; }
        public ICommand StopArchiveCommand { get; set; }
        #endregion

        #region Constructor

        public ArchiveViewModel(DataContext dataContext, UaClientApi uaClientApi)
        {
            _dataContext = dataContext;
            _uaClientApi = uaClientApi;

            LoadDataFromDataBase();
            InitializeTables();

            // TODO registrovat vsetky nody
            RegisterLoadedNodes();

            AddVariableToArchiveCommand = new RelayCommand(AddVariableToArchive);
            DeleteVariableFromArchiveCommand = new RelayCommand(DeleteVariableFromArchive);
            StartArchiveCommand = new RelayCommand(StartArchive);
            StopArchiveCommand = new RelayCommand(StopArchive);

            MessengerInstance.Register<SendSelectedRefNode>(
                this,
                node =>
                {
                    _refDescOfSelectedNode = node.RefNode;
                    AddArchiveVariableIsEnabled = _refDescOfSelectedNode.NodeClass == NodeClass.Variable;
                });
        }

        #endregion

        #region Command Methods

        private void StartArchive()
        {
            if (SelectedArchiveInfo == null) return;
            if (SelectedArchiveInfo.Running) return;

            if (SelectedArchiveInfo.ArchiveInterval == ArchiveInterval.None)
            {
                // TODO spusti subscription
                return;
            } 

            var interval = SelectedArchiveInfo.ArchiveInterval;
            var timer = new Timer(Archive, interval,TimeSpan.FromSeconds(1),TimeSpan.FromSeconds((int)interval));
            _timers.Add(interval, timer);
            SelectedArchiveInfo.Running = true;
        }

        private void StopArchive()
        {
            if (SelectedArchiveInfo == null) return;
            if (!SelectedArchiveInfo.Running) return;

            if (SelectedArchiveInfo.ArchiveInterval == ArchiveInterval.None)
            {
                // TODO stopnut subscription
                return;
            }

            var interval = SelectedArchiveInfo.ArchiveInterval;
            _timers.TryGetValue(interval, out var timer);

            if (timer == null) return;
            timer.Dispose();
            _timers.Remove(interval);
            SelectedArchiveInfo.Running = false;
        }

        private void AddVariableToArchive()
        {
            if (_refDescOfSelectedNode == null || SelectedArchiveInfo == null || _refDescOfSelectedNode.NodeClass != NodeClass.Variable) return;
            if (SelectedArchiveInfo.Running) return;

            var nodeId = _refDescOfSelectedNode.NodeId.ToString();
            var type = _uaClientApi.GetBuiltInTypeOfVariableNodeId(nodeId);

            var tmp = new VariableEntity()
            {
                Archive = SelectedArchiveInfo.ArchiveInterval,
                Name = nodeId,
                DataType = type,
            };
            _dataContext.Variables.Add(tmp);
            if (_dataContext.SaveChanges() != 1) return;

            var registeredNode = _uaClientApi.RegisterNode(nodeId);
            _registeredNodesForRead.Add( new ArchiveReadVariableModel()
            {
                Interval = SelectedArchiveInfo.ArchiveInterval,
                RegisteredNodeId = registeredNode,
                Type = type,
                VariableId = tmp.Id
            });
            ArchiveVariables.Add(tmp);

            SelectedArchiveInfo.VariablesCount++;
        }

        private void DeleteVariableFromArchive()
        {
            if (SelectedArchiveVariable == null) return;
            foreach (var archive in ArchiveInfo)
            {
                if (archive.ArchiveInterval == SelectedArchiveVariable.Archive && archive.Running) return;

                if (archive.ArchiveInterval == SelectedArchiveVariable.Archive && !archive.Running)
                    archive.VariablesCount--;
            }

            // vymazanie z databaze
            _dataContext.Variables.Remove(SelectedArchiveVariable);
            _dataContext.SaveChanges();

            var index = ArchiveVariables.IndexOf(SelectedArchiveVariable);
            // Vymazanie z tabulky
            ArchiveVariables.Remove(SelectedArchiveVariable);
            // Odregistrovanie
            _uaClientApi.UnRegisterNode(_registeredNodesForRead[index].RegisteredNodeId);
            // Vymazanie z nodes for read
            _registeredNodesForRead.RemoveAt(index);
        }
        #endregion

        #region Private Methods

        private void Archive(object objectInfo)
        {
            var interval = (ArchiveInterval)objectInfo;
            if (!IsTimerAlive(interval)) return;

            // TODO keby nahodou pridal pocas archivacie premennu tak sa nacita ?? prerobit to ??
            // Vytriedenie premennych pre tento interval a ancitanie hodnot
            var variablesForRead = _registeredNodesForRead.Where(x => x.Interval == interval).ToList();
            _uaClientApi.ReadValues(ref variablesForRead);

            // Vytvorenie zaznamov
            var records = variablesForRead.Select(x => new RecordEntity()
            {
                VariableEntityID = x.VariableId,
                Value = x.Value.ToString(),
                ArchiveTime = DateTime.Now
            }).ToList();

            // Archivacia
            _dataContext.Records.AddRange(records);
            MessengerInstance.Send(new SendArchivedValue(1));
            _dataContext.SaveChanges();
            
            // TODO ako sa disposuje session
        }

        private bool IsTimerAlive(ArchiveInterval interval)
        {
            _timers.TryGetValue(interval, out var timer);
            return timer != null;
        }

        private void LoadDataFromDataBase()
        {
            ArchiveVariables = new ObservableCollection<VariableEntity>(_dataContext.Variables.ToList());
        }

        private void RegisterLoadedNodes()
        {
            var nodeIds = ArchiveVariables.Select(x => x.Name).ToList();
            var registeredNodes =_uaClientApi.RegisterNodes(nodeIds);

            // TODO prerobit
            if(registeredNodes.Count != ArchiveVariables.Count) throw  new ValidationException("Pocty sa nerovnaju");

            _registeredNodesForRead = ArchiveVariables.Zip(registeredNodes, (entity, regNode) => new ArchiveReadVariableModel()
            {
                VariableId = entity.Id,
                RegisteredNodeId = regNode,
                Type = entity.DataType,
                Interval = entity.Archive
            }).ToList();
        }

        private void InitializeTables()
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
    }
}
