using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private readonly object _subscriptionLock = new object();
        private Dictionary<Type, List<IMessageSubscription>> _subscriptionsByType;

        #endregion Private Fields

        #region Static Default

        private static object _creationLock = new object();
        private static IMessenger _default;

        /// <summary>
        /// Get a default instance of the <see cref="Messenger"/> class.
        /// </summary>
        public static IMessenger Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_creationLock)
                    {
                        if (_default == null)
                        {
                            _default = new Messenger();
                        }
                    }
                }
                return _default;
            }
        }

        #endregion Static Default

        #region Constructors

        /// <summary>
        /// Create a new default instance of <see cref="Messenger"/>.
        /// </summary>
        public Messenger()
        {
            _subscriptionsByType = new Dictionary<Type, List<IMessageSubscription>>();
        }

        #endregion Constructors

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
        /// <param name="receiveDerivedMessages">Indicates the subscriber's
        /// desire to receieve or not receieve message which are derived
        /// from the subscribed message type.</param>
        /// <param name="action">The message handler to execute whenever
        /// the appropriate message type is sent.</param>
        public void Subscribe<TMessage>(object recipientToken, bool receiveDerivedMessages, Action<TMessage> action)
            where TMessage : IMessage
        {
            if (recipientToken == null)
                throw new ArgumentNullException("recipientToken");
            if (action == null)
                throw new ArgumentNullException("action");

            lock (_subscriptionLock)
            {
                Type messageType = typeof(TMessage);

                List<IMessageSubscription> currentSubscriptions = null;
                if (!_subscriptionsByType.TryGetValue(messageType, out currentSubscriptions))
                {
                    currentSubscriptions = new List<IMessageSubscription>();
                    _subscriptionsByType[messageType] = currentSubscriptions;
                }

                currentSubscriptions.Add(new MessageSubscription<TMessage>(recipientToken, action, receiveDerivedMessages));
            }
            CleanupSubscriptions();
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
            if (recipientToken == null)
                throw new ArgumentNullException("recipientToken");

            lock (_subscriptionLock)
            {
                Type messageType = typeof(TMessage);

                List<IMessageSubscription> currentSubscriptions = null;
                if (_subscriptionsByType.TryGetValue(messageType, out currentSubscriptions))
                {
                    var toRemoveList = (from sub in currentSubscriptions
                                        where object.ReferenceEquals(sub.RecipientToken, recipientToken)
                                        select sub).ToList();

                    foreach (var sub in toRemoveList)
                    {
                        currentSubscriptions.Remove(sub);
                    }
                }
            }
            CleanupSubscriptions();
        }

        /// <summary>
        /// Broadcast the given <see cref="IMessage"/> to all registered listeners.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to deliver.</typeparam>
        /// <param name="message">The message to send.</param>
        public void Send<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var subscriptionsToDeliver = GetSubscriptionsForMessage(message);
            foreach (var sub in subscriptionsToDeliver)
            {
                sub.DeliverMessage(message);
            }
            CleanupSubscriptions();
        }

        /// <summary>
        /// Broadcast the given <see cref="IMessage"/> to all registered listeners asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to deliver.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <returns>Returns the asynchronous task created for delivering the message.</returns>
        public Task SendAsync<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            return Task.Run(() => Send<TMessage>(message));
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Collect all subscriptions relevant to the given message,
        /// accounting for subscribers who wish to receive subtypes.
        /// </summary>
        /// <param name="message">The message for which to collect subscriptions.</param>
        /// <returns>Returns the list of all subscriptions for the given message type.</returns>
        private IEnumerable<IMessageSubscription> GetSubscriptionsForMessage<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Type messageType = message.GetType();
            List<IMessageSubscription> subscriptions;
            lock (_subscriptionLock)
            {
                subscriptions = (from subscriptionList in _subscriptionsByType.Values
                                 from subscription in subscriptionList
                                 where (subscription.IsSubscriberAlive
                                    && subscription.IsCompatibleMessage(message))
                                 select subscription).ToList();
            }
            return subscriptions;
        }

        /// <summary>
        /// Sweep through the current list of all subscriptions and remove any whose RecipientToken is expired.
        /// </summary>
        protected void CleanupSubscriptions()
        {
            lock (_subscriptionLock)
            {
                foreach (var subscriptionList in _subscriptionsByType.Values)
                {
                    var toRemoveList = (from sub in subscriptionList
                                        where !sub.IsSubscriberAlive
                                        select sub).ToList();

                    foreach (var sub in toRemoveList)
                    {
                        subscriptionList.Remove(sub);
                    }
                }
            }
        }

        #endregion Protected Methods

        /// <summary>
        /// Internal interface use for storing subscription information
        /// and delivering messages to individual subscribers.
        /// </summary>
        internal interface IMessageSubscription
        {
            /// <summary>
            /// Deliver the message to the subscribed recipient.
            /// </summary>
            /// <param name="message">The message to deliver.</param>
            void DeliverMessage(IMessage message);

            /// <summary>
            /// Determine whether or not the given message matches
            /// the subscriber's intended message type.
            /// </summary>
            /// <param name="message">The message whose type should be compared.</param>
            /// <returns>Returns True if the message is an exact match with
            /// the specified/subscribed type.  Also returns True if the message
            /// type is derived from the intended message type AND the subscriber
            /// opted to receive derived message types.
            /// Otherwise, returns false.</returns>
            bool IsCompatibleMessage(IMessage message);

            /// <summary>
            /// Get the value indicating whether the original subscriber
            /// is still alive, or if the subscription is no longer valid.
            /// </summary>
            bool IsSubscriberAlive { get; }

            /// <summary>
            /// Get the token used for tracking the recipient's subscription
            /// and lifetime.
            /// </summary>
            object RecipientToken { get; }
        }

        /// <summary>
        /// Internal class used for storing subscription information.
        /// </summary>
        internal class MessageSubscription<TMessage> : IMessageSubscription
            where TMessage : IMessage
        {
            #region Private Fields

            private Action<TMessage> _deliveryAction;
            private WeakReference _recipientToken;
            private bool _allowDerivedTypes;

            #endregion Private Fields

            #region Public Methods

            /// <summary>
            /// Creates a new <see cref="MessageSubscription{T}"/>, storing the given
            /// reipient token, delivery action, and flag indicating the subscriber's
            /// desire to receieve derived types.
            /// </summary>
            /// <param name="recipientToken">The token used to track the recipient's lifetime and the
            /// validity of the message subscription.</param>
            /// <param name="deliveryAction">The action to invoke when delivering the message.</param>
            /// <param name="allowDerivedTypes">Indicates the subscriber's desire to receive
            /// or not receive derived types of the base message type.</param>
            public MessageSubscription(object recipientToken, Action<TMessage> deliveryAction, bool allowDerivedTypes)
            {
                _recipientToken = new WeakReference(recipientToken);
                _deliveryAction = deliveryAction;
                _allowDerivedTypes = allowDerivedTypes;
            }

            /// <summary>
            /// Deliver the message to the subscribed recipient.
            /// </summary>
            /// <param name="message">The message to deliver.</param>
            public void DeliverMessage(IMessage message)
            {
                if (_deliveryAction != null)
                    _deliveryAction((TMessage)message);
            }

            /// <summary>
            /// Determine whether or not the given message matches
            /// the subscriber's intended message type.
            /// </summary>
            /// <param name="message">The message whose type should be compared.</param>
            /// <returns>Returns True if the message is an exact match with
            /// the specified/subscribed type.  Also returns True if the message
            /// type is derived from the intended message type AND the subscriber
            /// opted to receive derived message types.
            /// Otherwise, returns false.</returns>
            public bool IsCompatibleMessage(IMessage message)
            {
                Type currentMessageType = message.GetType();
                if (currentMessageType == typeof(TMessage))
                    return true;

                if (_allowDerivedTypes
                    && IsDerivativeType(currentMessageType))
                    return true;

                return false;
            }

            #endregion Public Methods

            #region Public Properties

            /// <summary>
            /// Get the value indicating whether the original subscriber
            /// is still alive, or if the subscription is no longer valid.
            /// </summary>
            public bool IsSubscriberAlive
            {
                get { return _recipientToken.Target != null; }
            }

            /// <summary>
            /// Get the token used for tracking the recipient's subscription
            /// and lifetime.
            /// </summary>
            public object RecipientToken
            {
                get { return _recipientToken.Target; }
            }

            /// <summary>
            /// Helper class determines if the given type is a subclass of
            /// the desired Message type, or if the given type implements
            /// the desired Message type's interface.
            /// </summary>
            /// <param name="type">The type to compare against the desired type.</param>
            /// <returns>Returns True if the given type is a subclass
            /// of the desired type, of if the given type implements the
            /// desired type's interface.  Otherwise, returns False.</returns>
            private bool IsDerivativeType(Type type)
            {
                Type desiredType = typeof(TMessage);

                if (type.GetTypeInfo().IsSubclassOf(desiredType))
                    return true;
                if (type.GetTypeInfo().ImplementedInterfaces.Contains(desiredType))
                    return true;

                return false;
            }

            #endregion Public Properties
        }
    }
}