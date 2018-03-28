﻿using GalaSoft.MvvmLight.Messaging;
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
        /// A shortcut to access the <see cref="DataContext"/>
        /// </summary>
        public static DataContext DataContext => IoC.Get<DataContext>();

        /// <summary>
        /// A shortcut to access the <see cref="Messenger"/>
        /// </summary>
        public static Messenger Messenger => IoC.Get<Messenger>();

        #endregion

        #region Setup

        public static void Configure()
        {
            // Bind all required view models
            BindViewModels();
            BindUaApi();
            BindDataContext();
            BindMessenger();
        }

        private static void BindMessenger()
        {
            Kernel.Bind<Messenger>().ToConstant(new Messenger());
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
            Kernel.Bind<UaClientApi>().ToConstant(new UaClientApi());
        }

        private static void BindDataContext()
        {
            // Bind to a single instance of Data Context
            Kernel.Bind<DataContext>().ToConstant(new DataContext());
        }

        public static void DisposeAll()
        {
            UaClientApi?.Disconnect();
            DataContext?.Dispose();
            Messenger?.Cleanup();
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
