using Ninject;

namespace OpcUa.Client.Core
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
        /// A shortcut to access the <see cref="UnitOfWork"/>
        /// </summary>
        public static IUnitOfWork UnitOfWork => IoC.Get<IUnitOfWork>();

        /// <summary>
        /// A shortcut to access the <see cref="Messenger"/>
        /// </summary>
        public static Messenger Messenger => IoC.Get<Messenger>();

        /// <summary>
        /// A shortcut to access the <see cref="Messenger"/>
        /// </summary>
        public static AppManager AppManager => IoC.Get<AppManager>();

        /// <summary>
        /// A shortcut to access the <see cref="IUIManager"/>
        /// </summary>
        public static IUIManager Ui => IoC.Get<IUIManager>();

        #endregion

        #region Setup
        public static void Configure()
        {
            BindViewModels();
            BindMessenger();
            BindUaClientApi();
            BindUnitOfWork();
            BindAppManager();
        }

        /// <summary>
        /// ApplicationViewModel handle changing pages of application
        /// </summary>
        private static void BindViewModels()
        {
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }

        /// <summary>
        /// Messenger handle communication between view models
        /// </summary>
        private static void BindMessenger()
        {
            Kernel.Bind<Messenger>().ToConstant(new Messenger());
        }

        /// <summary>
        /// UaClientApi handle communication with OPC UA Server
        /// </summary>
        private static void BindUaClientApi()
        {
            Kernel.Bind<UaClientApi>().ToConstant(new UaClientApi());
        }

        /// <summary>
        /// IUnitOfWork handle work with database and database entities
        /// </summary>
        private static void BindUnitOfWork()
        {
            Kernel.Bind<IUnitOfWork>().ToConstant(new UnitOfWork(new DataContext()));
        }

        /// <summary>
        /// AppManager handle state of application
        /// </summary>
        private static void BindAppManager()
        {
            Kernel.Bind<AppManager>().ToConstant(new AppManager());
        }

        /// <summary>
        /// Dispose all connections and database
        /// </summary>
        public static void DisposeAll()
        {
            UaClientApi.Disconnect();
            UnitOfWork.Dispose();
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