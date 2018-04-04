using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly UaClientApi _uaClientApi;
        private readonly IUnitOfWork _unitOfWork;

        #region Public Properties

        public string WelcomeText { get; set; } = "Lorem ipsum dolor, lorem isum dolor, Lorem ipsum dolor sit amet, lorem ipsum dolor sit amet";
        public ObservableCollection<ProjectModel> Projects { get; set; }
        public ProjectModel SelectedProject { get; set; }

        #endregion

        #region Commands

        public ICommand LoadProjectCommand { get; set; }
        public ICommand CreateProjectCommand { get; set; }
        
        #endregion

        public WelcomeViewModel()
        {
            // TODO vsetkych pages predavat IoC veci cey konstruktor z Page Convertora !
            _uaClientApi = IoC.UaClientApi;
            _unitOfWork = IoC.UnitOfWork;

            LoadProjects();

            LoadProjectCommand = new RelayCommand(LoadProject);
            CreateProjectCommand = new RelayCommand(() => IoC.Application.GoToPage(ApplicationPage.Endpoints));

            MessengerInstance.Register<SendCredentials>(
                this,
                msg => Login(msg.UserName, msg.Password));
        }

        private void LoadProject()
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
                    IoC.Ui.ShowLogIn(new LogInDialogViewModel());
            }
            catch (Exception e)
            {
                IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
                {
                    Title = "Error",
                    Message = e.Message,
                    OkText = "ok",
                });
            }
        }

        private void Login(string userName, SecureString password)
        {
            try
            {
                var user = _unitOfWork.Auth.Login(userName, password);
                if (user == null)
                {
                    IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
                    {
                        Title = "Error",
                        Message = "Zadali ste zlé prihlasovacie údaje!",
                        OkText = "ok"
                    });
                }
                var endpoint = _unitOfWork.Endpoints.SingleOrDefault(x => x.Id == SelectedProject.EndpointId);
                _uaClientApi.Connect(Mapper.CreateEndpointDescription(endpoint), userName, SecureStringHelpers.Unsecure(password), SelectedProject.SessionName);
            }
            catch (Exception e)
            {

                IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
                {
                    Title = "Error",
                    Message = e.Message,
                    OkText = "ok"
                });
            }

            IoC.Application.GoToPage(ApplicationPage.Main);
        }

        private void LoadProjects()
        {
            var projectsEntities = _unitOfWork.Projects.GetAll();

            // TODO preco sa mi nenacita ta referencia na endpoint
            if (projectsEntities == null) return;

            foreach (var project in projectsEntities)
            {
                project.Endpoint = _unitOfWork.Endpoints.SingleOrDefault(x => x.Id == project.EndpointId);
            }

            Projects = new ObservableCollection<ProjectModel>(Mapper.ProjectToListModel(projectsEntities));
        }
    }
}
