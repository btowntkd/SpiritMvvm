using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMVVM.Messaging
{
    /// <summary>
    /// Base interface for decoupled messages using the <see cref="IMessenger"/> interface.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Get the Sender of the message.
        /// </summary>
        object Sender { get; }
    }
}
