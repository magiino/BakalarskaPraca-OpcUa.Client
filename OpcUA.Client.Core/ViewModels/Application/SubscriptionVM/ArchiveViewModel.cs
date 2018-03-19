using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Opc.Ua;
using OpcUA.Client.Core.Models;

namespace OpcUA.Client.Core
{
    public class ArchiveViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly DataContext _dataContext;
        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _refDiscOfSelectedNode;
        private readonly Dictionary<ArchiveInterval, Timer> _timers = new Dictionary<ArchiveInterval, Timer>();
        private List<ArchiveReadVariable> _registeredNodesForRead;

        #endregion

        #region Public Properties

        public ObservableCollection<VariableEntity> ArchiveVariables { get; set; }
        public Variable SelectedArchiveVariable { get; set; }
       
        public ObservableCollection<ArchiveInfoTable> ArchiveInfo { get; set; }
        public ArchiveInfoTable SelectedArchiveInfo { get; set; }

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
                    _refDiscOfSelectedNode = node.RefNode;
                    AddArchiveVariableIsEnabled = _refDiscOfSelectedNode.NodeClass == NodeClass.Variable;
                });
        }

        #endregion

        #region Command Methods

        private void StartArchive()
        {
            if (SelectedArchiveInfo == null) return;
            var interval = SelectedArchiveInfo.ArchiveInterval;
            var timer = new Timer(Archive, interval,TimeSpan.FromSeconds(1),TimeSpan.FromSeconds((int)interval));
            _timers.Add(interval, timer);
        }

        private void StopArchive()
        {
            var interval = SelectedArchiveInfo.ArchiveInterval;
            _timers.TryGetValue(interval, out var timer);

            if (timer == null) return;
            timer.Dispose();
            _timers.Remove(interval);

        }

        private void AddVariableToArchive()
        {
            if (_refDiscOfSelectedNode == null || SelectedArchiveInfo == null) return;

            var nodeId = _refDiscOfSelectedNode.NodeId.ToString();
            var type = _uaClientApi.GetBuiltInTypeOfVariableNodeId(nodeId);

            var tmp = new VariableEntity()
            {
                Archive = SelectedArchiveInfo.ArchiveInterval,
                Name = nodeId,
                DataType = type,
            };
            _dataContext.Variables.Add(tmp);
            if (_dataContext.SaveChanges() == 1)
                ArchiveVariables.Add(tmp);

            // TODO Zaregistrovat premennu a pridat ju medzi ostatne
        }

        // TODO DeleteVariableFromArchive
        private void DeleteVariableFromArchive()
        {
            if (SelectedArchiveVariable == null) return;
        }
        #endregion

        #region Private Methods

        private void Archive(object objectInfo)
        {
            var interval = (ArchiveInterval)objectInfo;
            if (!IsTimerAlive(interval)) return;

            // TODO nacitat hodnoty
            // TODO keby nahodou pridal pocas archivacie premennu tak sa nacita ?? prerobit to ??
            var variablesForRead = _registeredNodesForRead.Where(x => x.Interval == interval).ToList();
            _uaClientApi.ReadValues(ref variablesForRead);

            var records = variablesForRead.Select(x => new RecordEntity()
            {
                VariableEntityID = x.VariableId,
                Value = x.Value.ToString(),
                ArchiveTime = DateTime.Now
            }).ToList();

            // TODO archivovat
            _dataContext.Records.AddRange(records);
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

            _registeredNodesForRead = ArchiveVariables.Zip(registeredNodes, (entity, regNode) => new ArchiveReadVariable()
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
            ArchiveInfo = new ObservableCollection<ArchiveInfoTable>();

            foreach (var interval in archiveIntervals)
            {
                ArchiveInfo.Add(new ArchiveInfoTable()
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
