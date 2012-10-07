using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMVVM.Messaging
{
    /// <summary>
    /// Default implementation of the <see cref="IMessenger"/> interface,
    /// facilitating the delivery of messages in a decoupled fashion.
    /// </summary>
    public class Messenger : IMessenger
    {
        #region Private Fields

        private readonly object _subscriberLock = new object();
        private Dictionary<Type, List<MessageSubscription>> _subscribers;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new default instance of <see cref="Messenger"/>.
        /// </summary>
        public Messenger()
        {
            _subscribers = new Dictionary<Type, List<MessageSubscription>>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified message type, using the given
        /// 'token' as the recipient lookup, and assigning the given
        /// message handler.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message for which
        /// to subscribe.</typeparam>
        /// <param name="recipientToken">The object to use as a key when
        /// unsubscribing from the message at a later time, and when keeping
        /// track of object lifetime using <see cref="WeakReference"/>s.</param>
        /// <param name="action">The message handler to execute whenever
        /// the appropriate message type is sent.</param>
        public void Subscribe<TMessage>(object recipientToken, Action<TMessage> action)
            where TMessage : IMessage
        {
            Subscribe<TMessage>(recipientToken, false, action);
        }

        /// <summary>
        /// Subscribe to the specified message type, using the given
        /// 'token' as the recipient lookup, and assigning the given
        /// message handler.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message for which
        /// to subscribe.</typeparam>
        /// <param name="recipientToken">The object to use as a key when
        /// unsubscribing from the message at a later time, and when keeping
        /// track of object lifetime using <see cref="WeakReference"/>s.</param>
        /// <param name="receieveDerivedMessages">Indicates the subscriber's
        /// desire to receieve or not receieve message which are derived
        /// from the subscribed message type.</param>
        /// <param name="action">The message handler to execute whenever
        /// the appropriate message type is sent.</param>
        public void Subscribe<TMessage>(object recipientToken, bool receieveDerivedMessages, Action<TMessage> action)
            where TMessage : IMessage
        {

        }

        /// <summary>
        /// Unsubscribe the given 'token' from receiving the specified message type.
        /// </summary>
        /// <remarks>
        /// No exception is thrown if the recipientToken could not be found.
        /// </remarks>
        /// <typeparam name="TMessage">THe type of the message for which to unsubscribe.</typeparam>
        /// <param name="recipientToken">The lookup token provided during registration.</param>
        public void Unsubscribe<TMessage>(object recipientToken)
            where TMessage : IMessage
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Broadcast the given <see cref="IMessage"/> to all registered listeners.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to deliver.</typeparam>
        /// <param name="message">The message to send.</param>
        public void Send<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Broadcast the given <see cref="IMessage"/> to all registered listeners asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to deliver.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <returns>Returns the asynchronous task created for delivering the message.</returns>
        public Task SendAsync<TMessage>(TMessage message) where TMessage : IMessage
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Internal class used for storing subscription information.
        /// </summary>
        internal class MessageSubscription
        {
            /// <summary>
            /// Get the <see cref="Action"/> to be invoked upon delivery of the message.
            /// </summary>
            public Action<IMessage> DeliveryAction { get; private set; }

            /// <summary>
            /// Get a <see cref="WeakReference"/> to the recipient token,
            /// used to track recipient lifetime and determine validity
            /// of the subscription.
            /// </summary>
            public WeakReference RecipientToken { get; private set; }

            /// <summary>
            /// Get the value indicating whether or not the recipient
            /// would like to receive derived types of the subscribed
            /// message type.
            /// </summary>
            public bool ReceiveDerivedTypes { get; private set; }

            /// <summary>
            /// Creates a new <see cref="MessageSubscription"/>, storing the given
            /// reipient token, delivery action, and flag indicating the subscriber's
            /// desire to receieve derived types.
            /// </summary>
            /// <param name="recipientToken">The token used to track the recipient's lifetime and the
            /// validity of the message subscription.</param>
            /// <param name="deliveryAction">The action to invoke when delivering the message.</param>
            /// <param name="receiveDerivedTypes">Indicates the subscriber's desire to receive
            /// or not receive derived types of the base message type.</param>
            public MessageSubscription(object recipientToken, Action<IMessage> deliveryAction, bool receiveDerivedTypes)
            {
                RecipientToken = new WeakReference(recipientToken);
                DeliveryAction = deliveryAction;
                ReceiveDerivedTypes = receiveDerivedTypes;
            }
        }
    }
}
