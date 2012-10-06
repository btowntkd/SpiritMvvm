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
    }
}
