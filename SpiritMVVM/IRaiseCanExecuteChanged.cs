using System.Windows.Input;

namespace SpiritMVVM
{
    /// <summary>
    /// Interface which provides a mechanism for raising the
    /// <see cref="ICommand.CanExecuteChanged"/> event externally.
    /// </summary>
    public interface IRaiseCanExecuteChanged : ICommand
    {
        /// <summary>
        /// Raise the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
