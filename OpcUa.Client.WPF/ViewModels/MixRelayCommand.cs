using System;
using System.Windows.Input;

namespace OpcUa.Client.WPF
{
    /// <inheritdoc />
    /// <summary>
    /// A basic command that runs an Action with condition for execute
    /// </summary>
    public class MixRelayCommand : ICommand
    {
        #region Private Members
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        #endregion

        #region Public Events
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        #endregion

        #region Constructor
        public MixRelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public MixRelayCommand(Action<object> execute) : this(execute, null) { }
        #endregion

        #region Command Methods
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion
    }
}