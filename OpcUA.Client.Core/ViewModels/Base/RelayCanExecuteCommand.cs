using System;
using System.Windows.Input;

namespace OpcUa.Client.Core
{
    /// <inheritdoc />
    /// <summary>
    /// A basic command that runs an Action with condition for execute
    /// </summary>
    public class RelayCanExecuteCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action _action;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCanExecuteCommand(Action action)
        {
            _action = action;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can execute if conndition is true
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _action();
        }

        #endregion
    }
}
