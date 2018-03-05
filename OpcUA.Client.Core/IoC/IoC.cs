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
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => IoC.Get<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access the <see cref="UaClient"/>
        /// </summary>
        public static UaClient UaClient => IoC.Get<UaClient>();

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
        }

        private static void BindUaApi()
        {
            // Bind to a single instance of UaClientHelperApi
            Kernel.Bind<UaClient>().ToConstant(new UaClient());
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
