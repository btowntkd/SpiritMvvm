using System;
using System.Windows.Input;

namespace SpiritMVVM
{
    /// <summary>
    /// The <see cref="RelayCommand"/> class assigns an <see cref="Action"/> delegate to the <see cref="Execute"/>
    /// method and a <see cref="Func{T}"/> delegate to the <see cref="CanExecute"/> method,
    /// allowing <see cref="ICommand"/> objects to be implemented completely from within a View-Model.
    /// </summary>
    public class RelayCommand : ICommand, IRaiseCanExecuteChanged, IReactOnDependencyChanged
    {
        #region Private Fields

        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new <see cref="RelayCommand"/> with the given execution delegate.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to execute when the
        /// <see cref="Execute"/> method is called.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        { }

        /// <summary>
        /// Create a new <see cref="RelayCommand"/> with the given execution delegate.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to execute when the
        /// <see cref="Execute"/> method is called.</param>
        /// <param name="canExecute">The <see cref="Func{T}"/> to execute
        /// when the <see cref="CanExecute"/> method is queried.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Implementation

        /// <summary>
        /// Event which is raised when the command's ability to be executed changes.
        /// </summary>
        public event EventHandler CanExecuteChanged = null;

        /// <summary>
        /// Determine if the command can be executed in its current state.
        /// </summary>
        /// <param name="parameter">An optional parameter.</param>
        /// <returns>Returns True if the command can be executed.  Otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            var canExecuteHandler = _canExecute;
            if (canExecuteHandler != null)
            {
                return canExecuteHandler();
            }

            return true;
        }

        /// <summary>
        /// Execute the command's delegate method.
        /// </summary>
        /// <param name="parameter">An optional parameter.</param>
        public void Execute(object parameter)
        {
            var executeHandler = _execute;
            if (executeHandler != null)
            {
                executeHandler();
            }
        }

        #endregion

        #region IRaiseCanExecuteChanged Implementation

        /// <summary>
        /// Raise the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IReactOnDependencyChanged Implementation

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event
        /// when a dependency changes.
        /// </summary>
        void IReactOnDependencyChanged.OnDependencyChanged()
        {
            RaiseCanExecuteChanged();
        }

        #endregion
    }
}
