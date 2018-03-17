using System.Windows.Input;

namespace OpcUA.Client.Core
{
    public class WelcomeViewModel : BaseViewModel
    {
        #region Public Properties

        public string WelcomeText { get; set; } =
            "Lorem ipsum dolor, lorem isum dolor, Lorem ipsum dolor sit amet, lorem ipsum dolor sit amet"; 

        #endregion

        #region Commands

        /// <summary>
        /// The command for creating new session
        /// </summary>
        public ICommand NewSessionCommand { get; set; }

        /// <summary>
        /// The command for load existing session
        /// </summary>
        public ICommand LoadSessionCommand { get; set; }

        #endregion

        public WelcomeViewModel()
        {
            NewSessionCommand = new RelayCommand(() => IoC.Application.GoToPage(ApplicationPage.Endpoints));
            LoadSessionCommand = new RelayCommand(() => IoC.Application.GoToPage(ApplicationPage.Main));
        }
    }
}
