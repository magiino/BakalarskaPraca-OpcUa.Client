using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Windows.Input;
using Opc.Ua;
using OpcUa.Client.Core;

namespace OpcUa.Client.WPF
{
    public class WelcomeViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly Messenger _messenger;
        private readonly UaClientApi _uaClientApi;
        private readonly IUnitOfWork _unitOfWork; 
        #endregion

        #region Public Properties
        public string WelcomeText { get; set; } = "This application allows you to connect to opc server and gather or observe data.";
        public ObservableCollection<ProjectModel> Projects { get; set; }
        public ProjectModel SelectedProject { get; set; }
        #endregion

        #region Commands
        public ICommand LoadProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand CreateProjectCommand { get; }
        #endregion

        #region Constructor
        public WelcomeViewModel(IUnitOfWork iUnitOfWork, UaClientApi uaClientApi, Messenger messenger)
        {
            _messenger = messenger;
            _uaClientApi = uaClientApi;
            _unitOfWork = iUnitOfWork;

            OnLoad();

            LoadProjectCommand = new MixRelayCommand(LoadProject, LoadAndDeleteProjectCanUse);
            DeleteProjectCommand = new MixRelayCommand(DeleteProject, LoadAndDeleteProjectCanUse);
            CreateProjectCommand = new MixRelayCommand((obj) => IoC.Application.GoToPage(ApplicationPage.Endpoints));

            _messenger.Register<SendCredentials>(msg => Login(msg.UserName, msg.Password));
        }
        #endregion

        #region Command Methods
        private void LoadProject(object parameter)
        {
            try
            {
                var endpoint = _unitOfWork.Endpoints.SingleOrDefault(x => x.Id == SelectedProject.EndpointId);
                if (SelectedProject.UserId == null)
                {
                    _uaClientApi.ConnectAnonymous(Mapper.CreateEndpointDescription(endpoint), SelectedProject.SessionName);
                    IoC.AppManager.ProjectId = SelectedProject.Id;
                    IoC.Application.GoToPage(ApplicationPage.Main);
                }
                else
                    IoC.Ui.ShowLogIn(new LogInDialogViewModel(_messenger));
            }
            catch (Exception e)
            {
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }
        }

        private void DeleteProject(object parameter)
        {
            var project = _unitOfWork.Projects.SingleOrDefault(x => x.Id == SelectedProject.Id);
            _unitOfWork.Endpoints.Remove(project.Endpoint);

            if (project.UserId != null)
                _unitOfWork.Auth.RemoveUser(project.UserId.Value);

            var notifications = _unitOfWork.Notifications.Find(x => x.ProjectId == SelectedProject.Id).ToList();
            _unitOfWork.Notifications.RemoveRange(notifications);

            var variables = _unitOfWork.Variables.Find(x => x.ProjectId == SelectedProject.Id).ToList();

            if (variables.Count != 0)
            {
                foreach (var variable in variables)
                    _unitOfWork.Variables.DeleteById(variable.Id);
            }

            Projects.Remove(SelectedProject);
        }
        #endregion

        #region CanUse Methods

        public bool LoadAndDeleteProjectCanUse(object parameter)
        {
            return SelectedProject != null;
        }
        #endregion

        #region Private Methods
        private void Login(string userName, SecureString password)
        {
            try
            {
                var user = _unitOfWork.Auth.Login(userName, password);
                if (user == null)
                {
                    IoC.AppManager.ShowWarningMessage("Zadali ste zlé prihlasovacie údaje!");
                    return;
                }
                var endpoint = _unitOfWork.Endpoints.SingleOrDefault(x => x.Id == SelectedProject.EndpointId);
                _uaClientApi.Connect(Mapper.CreateEndpointDescription(endpoint), userName, SecureStringHelpers.Unsecure(password), SelectedProject.SessionName);
                IoC.AppManager.ProjectId = SelectedProject.Id;
            }
            catch (Exception e)
            {
                Utils.Trace(Utils.TraceMasks.Error, $"{e.Message}");
                IoC.AppManager.ShowExceptionErrorMessage(e);
            }
            IoC.Application.GoToPage(ApplicationPage.Main);
        }

        private void OnLoad()
        {
            var projectsEntities = _unitOfWork.Projects.GetAllWithEndpoints();

            if (projectsEntities == null) return;
            Projects = new ObservableCollection<ProjectModel>(Mapper.ProjectEntityToProjectListModel(projectsEntities));
        } 
        #endregion
    }
}