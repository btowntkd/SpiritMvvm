using SpiritMVVM.Messaging;
using System.ComponentModel;

namespace SpiritMVVM
{
    /// <summary>
    /// Provides common ViewModel-related functionality.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Get or Set a messenger object used for loosely-coupled messaging.
        /// </summary>
        IMessenger Messenger { get; set; }
    }
}