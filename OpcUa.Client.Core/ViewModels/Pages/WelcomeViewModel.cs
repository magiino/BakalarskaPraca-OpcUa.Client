using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Windows.Input;

namespace OpcUa.Client.Core
{
    public class WelcomeViewModel : BaseViewModel
    {
        private UaClientApi _uaClientApi;
        private DataContext _dataContext;
        private IAuthRepository _authRepo;

        #region Public Properties

        public string WelcomeText { get; set; } = "Lorem ipsum dolor, lorem isum dolor, Lorem ipsum dolor sit amet, lorem ipsum dolor sit amet";
        public ObservableCollection<ProjectModel> Projects { get; set; }
        public ProjectModel SelectedProject { get; set; }

        #endregion

        #region Commands

        public ICommand CreateProjectCommand { get; set; }
        public ICommand LoadProjectCommand { get; set; }

        #endregion

        public WelcomeViewModel()
        {
            _uaClientApi = IoC.UaClientApi;
            // TODO repository pre project
            _dataContext = IoC.DataContext;
            _authRepo = new AuthRepository(_dataContext);
            Projects = LoadProjects(_dataContext);

            CreateProjectCommand = new RelayCommand(() => IoC.Application.GoToPage(ApplicationPage.Endpoints));
            LoadProjectCommand = new RelayCommand(LoadProject);

            MessengerInstance.Register<SendCredentials>(
                this,
                msg => Login(msg.UserName, msg.Password));
        }

        private void LoadProject()
        {
            try
            {
                var endpoint = _dataContext.Endpoints.SingleOrDefault(x => x.Id == SelectedProject.EndpointId);
                if (SelectedProject.UserId == null)
                {
                    _uaClientApi.ConnectAnonymous(Mapper.CreateEndpointDescription(endpoint), SelectedProject.SessionName);
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
                    OkText = "ok"
                });
            }
        }

        private void Login(string userName, SecureString password)
        {
            //// IMPORTANT: Never store unsecure password in variable like this
            //var pass = (parameter as IHavePassword).SecurePassword.Unsecure();

            try
            {
                var user = _authRepo.Login(userName, password);
                if (user == null)
                {
                    IoC.Ui.ShowMessage(new MessageBoxDialogViewModel()
                    {
                        Title = "Error",
                        Message = "Zadali ste zlé prihlasovacie údaje!",
                        OkText = "ok"
                    });
                }
                var endpoint = _dataContext.Endpoints.SingleOrDefault(x => x.Id == SelectedProject.EndpointId);
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

        private ObservableCollection<ProjectModel> LoadProjects(DataContext dataContext)
        {
            var projectsEntities = dataContext.Projects.ToList();
            projectsEntities.ForEach((project) =>
                {
                    project.Endpoint = dataContext.Endpoints.SingleOrDefault(x => x.Id == project.EndpointId);
                });
            return new ObservableCollection<ProjectModel>(Mapper.ProjectToListModel(projectsEntities));
        }
    }
}
