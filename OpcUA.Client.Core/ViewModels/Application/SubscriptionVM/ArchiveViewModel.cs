using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class ArchiveViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly DataContext _dataContext;
        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _refDiscOfSelectedNode;
        private Dictionary<ArchiveInterval, Timer> _timers = new Dictionary<ArchiveInterval, Timer>();

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
            var tmp = new VariableEntity()
            {
                Archive = SelectedArchiveInfo.ArchiveInterval,
                Name = nodeId,
                DataType = _uaClientApi.GetBuiltInTypeOfVariableNodeId(nodeId).ToString(),
            };
            _dataContext.Variables.Add(tmp);
            if (_dataContext.SaveChanges() == 1)
                ArchiveVariables.Add(tmp);
        }

        private void DeleteVariableFromArchive()
        {
            if (SelectedArchiveVariable == null) return;
        }
        #endregion

        #region Private Methods

        private void Archive(object objectInfo)
        {
            if (!IsTimerAlive((ArchiveInterval)objectInfo)) return;
            // TODO registrovat vsetky nody
            // TODO nacitat hodnoty
            // TODO archivovat
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
