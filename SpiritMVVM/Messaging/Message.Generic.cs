namespace SpiritMVVM.Messaging
{
    /// <summary>
    /// Generic Message type allows users to wrap content.
    /// </summary>
    /// <typeparam name="TContent">The type of the content.</typeparam>
    public class Message<TContent> : Message
    {
        /// <summary>
        /// Get the content of the message.
        /// </summary>
        public TContent Content { get; private set; }

        /// <summary>
        /// Default constructor creates a new instance of <see cref="Message"/>
        /// with a <see cref="Message.Sender"/> value of null.
        /// </summary>
        /// <param name="content">The content to include with the message.</param>
        public Message(TContent content)
            : base()
        {
            Content = content;
        }

        /// <summary>
        /// Create a new instance of <see cref="Message"/>, specifying the given
        /// object as the sender.
        /// </summary>
        /// <param name="sender">The object which originated the message.</param>
        /// <param name="content">The content to include with the message.</param>
        public Message(object sender, TContent content)
            : base(sender)
        {
            Content = content;
        }
    }
}