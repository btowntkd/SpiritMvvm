using SpiritMVVM.Messaging;

namespace SpiritMVVM.ViewModels
{
    /// <summary>
    /// A base implementation of the <see cref="IViewModel"/> interface,
    /// providing a default <see cref="IMessenger"/> instance for loosely-coupled messaging.
    /// </summary>
    public class ViewModelBase : ObservableObject, IViewModel
    {
        private IMessenger _messenger;

        /// <summary>
        /// Get or Set the <see cref="IMessenger"/> instance
        /// to use when performing loosely-coupled communication.
        /// </summary>
        public IMessenger Messenger
        {
            get { return _messenger; }
            set { Set(ref _messenger, value); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ViewModelBase()
        {
            Messenger = SpiritMVVM.Messaging.Messenger.Default;
        }
    }
}
