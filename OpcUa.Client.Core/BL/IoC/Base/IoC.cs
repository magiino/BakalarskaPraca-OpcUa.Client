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
            BindUaApi();
            BindUnitOfWork();
            BindStateManager();
            BindMessenger();
        }

        private static void BindViewModels()
        {
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }

        private static void BindUaApi()
        {
            Kernel.Bind<UaClientApi>().ToConstant(new UaClientApi());
        }

        private static void BindUnitOfWork()
        {
            Kernel.Bind<IUnitOfWork>().ToConstant(new UnitOfWork(new DataContext()));
        }

        private static void BindMessenger()
        {
            Kernel.Bind<Messenger>().ToConstant(new Messenger());
        }

        private static void BindStateManager()
        {
            Kernel.Bind<AppManager>().ToConstant(new AppManager());
        }

        public static void DisposeAll()
        {
            // TODO UaClientApi spravit ako repository do UnitOfWork
            UaClientApi?.Disconnect();
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
