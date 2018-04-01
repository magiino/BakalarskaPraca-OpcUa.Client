using System;
using System.Windows.Input;

namespace OpcUa.Client.Core
{
    /// <inheritdoc />
    /// <summary>
    /// A basic command that runs an Action with condition for execute
    /// </summary>
    public class MixRelayCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        ///// <summary>
        ///// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        ///// </summary>
        //public event EventHandler CanExecuteChanged
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MixRelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public MixRelayCommand(Action<object> execute) : this(execute, null) { }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can execute if conndition is true
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}
