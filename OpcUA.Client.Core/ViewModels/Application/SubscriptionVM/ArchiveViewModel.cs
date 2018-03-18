using System.Collections.ObjectModel;
using System.Windows.Input;
using Opc.Ua;

namespace OpcUA.Client.Core
{
    public class ArchiveViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly UaClientApi _uaClientApi;
        private ReferenceDescription _refDiscOfSelectedNode;

        #endregion

        #region Public Properties

        public ObservableCollection<Variable> ArchiveVariables { get; set; } = new ObservableCollection<Variable>();
        public Variable SelectedSubscribedVariable { get; set; }
        public bool DeleteIsEnabled => SelectedSubscribedVariable != null;
        public bool ArchiveCreated { get; set; }

        #endregion

        #region Commands

        public ICommand AddVariableToSubscriptionCommand { get; set; }
        public ICommand DeleteVariableFromSubscriptionCommand { get; set; }
        public ICommand CreateSubscriptionCommand { get; set; }
        public ICommand DeleteSubscriptionCommand { get; set; }

        #endregion

        #region Constructor

        public ArchiveViewModel(UaClientApi uaClientApi)
        {
            _uaClientApi = uaClientApi;

            AddVariableToSubscriptionCommand = new RelayCommand(AddVariableToArchive);
            DeleteVariableFromSubscriptionCommand = new RelayCommand(DeleteVariableFromArchive);
            CreateSubscriptionCommand = new RelayCommand(StartArchive);
            DeleteSubscriptionCommand = new RelayCommand(StopArchive);

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
    }
}
