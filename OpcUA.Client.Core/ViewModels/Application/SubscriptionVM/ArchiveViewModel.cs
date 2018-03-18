using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Ninject.Infrastructure.Language;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class ArchiveViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly DataContext _dataContext;
        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _refDiscOfSelectedNode;

        #endregion

        #region Public Properties

        public ObservableCollection<VariableEntity> ArchiveVariables { get; set; }
        public Variable SelectedSubscribedVariable { get; set; }
        public bool DeleteIsEnabled => SelectedSubscribedVariable != null;
        public bool ArchiveCreated { get; set; }
        public IEnumerable<ArchiveInterval> ArchiveIntervals { get; set; } = Enum.GetValues(typeof(ArchiveInterval)).Cast<ArchiveInterval>();
        public ObservableCollection<ArchiveInfoTable> ArchiveInfo { get; set; } = new ObservableCollection<ArchiveInfoTable>();

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
                });
        }



        #endregion

        #region Command Methods

        private void StartArchive()
        {
        
        }

        private void StopArchive()
        {
         
        }

        private void AddVariableToArchive()
        {
            if (_refDiscOfSelectedNode == null) return;
            var tmp = new Variable();

        }

        private void DeleteVariableFromArchive()
        {
            if (SelectedSubscribedVariable == null) return;
           
        }
        #endregion

        #region Private Methods

        private void LoadDataFromDataBase()
        {
            ArchiveVariables = new ObservableCollection<VariableEntity>(_dataContext.Variables.ToList());
        }

        private void InitializeTables()
        {
            foreach (var interval in ArchiveIntervals)
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
