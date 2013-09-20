using System;
using System.Threading.Tasks;

namespace SpiritMVVM.Messaging
{
    /// <summary>
    /// The <see cref="IMessenger"/> interface allows objects to exchange messages
    /// in a decoupled fashion.
    /// </summary>
    public interface IMessenger
    {
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
        void Subscribe<TMessage>(object recipientToken, Action<TMessage> action)
            where TMessage : IMessage;

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
        void Subscribe<TMessage>(object recipientToken, bool receiveDerivedMessages, Action<TMessage> action)
            where TMessage : IMessage;

        /// <summary>
        /// Unsubscribe the given 'token' from receiving the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">THe type of the message for which to unsubscribe.</typeparam>
        /// <param name="recipientToken">The lookup token provided during registration.</param>
        void Unsubscribe<TMessage>(object recipientToken)
            where TMessage : IMessage;

        /// <summary>
        /// Broadcast the given <see cref="IMessage"/> to all registered listeners.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to deliver.</typeparam>
        /// <param name="message">The message to send.</param>
        void Send<TMessage>(TMessage message)
            where TMessage : IMessage;

        /// <summary>
        /// Broadcast the given <see cref="IMessage"/> to all registered listeners asynchronously.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to deliver.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <returns>Returns the asynchronous task created for delivering the message.</returns>
        Task SendAsync<TMessage>(TMessage message)
            where TMessage : IMessage;
    }
}