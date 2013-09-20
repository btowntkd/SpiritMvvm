using SpiritMVVM.Messaging;

namespace SpiritMVVM.ViewModels
{
    /// <summary>
    /// A base implementation of the <see cref="IViewModel"/> interface,
    /// providing a default <see cref="IMessenger"/> instance for loosely-coupled messaging.
    /// </summary>
    /// <remarks>
    /// The <see cref="ViewModelBase"/> class provides an <see cref="OnMessengerChanged"/>
    /// method to override, allowing users to unsubscribe from the old
    /// <see cref="IMessenger"/> instance and subscribe to the new
    /// <see cref="IMessenger"/> instance.
    /// </remarks>
    public abstract class ViewModelBase : ObservableObject, IViewModel
    {
        private IMessenger _messenger;

        /// <summary>
        /// Get or Set the <see cref="IMessenger"/> instance
        /// to use when performing loosely-coupled communication.
        /// </summary>
        public IMessenger Messenger
        {
            get { return _messenger; }
            set { Set(ref _messenger, value, OnMessengerChanged); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ViewModelBase()
            : this(SpiritMVVM.Messaging.Messenger.Default)
        { }

        /// <summary>
        /// Constructor which specifies the <see cref="IMessenger"/> to assign the
        /// <see cref="ViewModelBase.Messenger"/> property.
        /// </summary>
        /// <param name="messenger">The messenger to use when broadcasting messages.</param>
        public ViewModelBase(IMessenger messenger)
        {
            // Assign the value to the property wrapper,
            // to ensure that the "OnMessengerChanged" method is called.
            Messenger = messenger;
        }

        /// <summary>
        /// Executed whenever the <see cref="ViewModelBase.Messenger"/> property
        /// is changed, allowing users to unsubscribe from the old <see cref="IMessenger"/>
        /// and subscribe to messages from the new one.
        /// </summary>
        /// <param name="oldValue">The previous value of <see cref="ViewModelBase.Messenger"/>.</param>
        /// <param name="newValue">The new value of <see cref="ViewModelBase.Messenger"/>.</param>
        protected virtual void OnMessengerChanged(IMessenger oldValue, IMessenger newValue)
        {
        }
    }
}