using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// Subscribe to the given message type with the given message handler.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message for which
        /// to subscribe.</typeparam>
        /// <param name="action">The message handler to execute whenever
        /// the appropriate message type is sent.</param>
        void Subscribe<TMessage>(Action<TMessage> action);

        void Unsubscribe

        /// <summary>
        /// Broadcast the given <see cref="IMessage"/> to all registered listeners.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        void Send<TMessage>(TMessage message) where TMessage : IMessage;

        /// <summary>
        /// Broadcase the given <see cref="IMessage"/> to all registered listeners asynchronously.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendAsync<TMessage>(TMessage message) where TMessage : IMessage;
    }
}
