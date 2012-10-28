using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpiritMVVM.Utils
{
    /// <summary>
    /// Interface for a portable dispatcher, capable of invoking
    /// methods on a specific synchronization context, such as the UI.
    /// </summary>
    public interface IPortableDispatcher
    {
        /// <summary>
        /// Get or Set the <see cref="SynchronizationContext"/> for the current instance.
        /// </summary>
        SynchronizationContext Context { get; set; }

        /// <summary>
        /// Post the given action to the associated <see cref="SynchronizationContext"/>
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <returns>Returns a <see cref="Task"/> associated with the executing action.</returns>
        Task BeginInvoke(Action action);
    }
}
