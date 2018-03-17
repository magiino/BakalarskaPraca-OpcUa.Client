using Ninject;

namespace OpcUA.Client.Core
{
    /// <summary>
    /// The IoC container for our application
    /// </summary>
    public static class IoC
    {
        #region Public Properties

        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        public static IKernel Kernel { get; } = new StandardKernel();

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => IoC.Get<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="UaClientApi"/>
        /// </summary>
        public static UaClientApi UaClientApi => IoC.Get<UaClientApi>();

        /// <summary>
        /// A shortcut to access the <see cref="UaClientApi"/>
        /// </summary>
        //public static ApplicationManager ApplicationManager => IoC.Get<ApplicationManager>();

        #endregion

        #region Setup

        public static void Configure()
        {
            // Bind all required view models
            BindViewModels();
            BindUaApi();
        }

        /// <summary>
        /// Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            // Bind to a single instance of Application view model
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());

            //Kernel.Bind<MessageListener>().ToConstant(new MessageListener());
        }

        private static void BindUaApi()
        {
            // Bind to a single instance of UaClientHelperApi
            Kernel.Bind<UaClientApi>().ToConstant(new UaClientApi());
            //Kernel.Bind<ApplicationManager>().ToConstant(new ApplicationManager(new UaClientApi()));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get's a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        #endregion
    }
}
