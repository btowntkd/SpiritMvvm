namespace SpiritMVVM.Messaging
{
    /// <summary>
    /// Base class for decoupled messages using the <see cref="IMessenger"/> interface.
    /// </summary>
    public class Message : IMessage
    {
        /// <summary>
        /// Get the object acting as the originator of the message.
        /// </summary>
        public object Sender { get; private set; }

        /// <summary>
        /// Default constructor creates a new instance of <see cref="Message"/>
        /// with a <see cref="Message.Sender"/> value of null.
        /// </summary>
        public Message()
        {
            Sender = null;
        }

        /// <summary>
        /// Create a new instance of <see cref="Message"/>, specifying the given
        /// object as the sender.
        /// </summary>
        /// <param name="sender">The object which originated the message.</param>
        public Message(object sender)
        {
            Sender = sender;
        }
    }
}