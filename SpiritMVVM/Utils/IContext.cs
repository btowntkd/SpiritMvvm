using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMVVM.Utils
{
    /// <summary>
    /// Common interface for dispatching actions across threads.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Determine if the current <see cref="IContext"/> 
        /// exists in the currently-running thread.
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Execute the specified <see cref="Action"/> synchronously 
        /// in the current <see cref="IContext"/>, returning once
        /// the action has completed.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        void Invoke(Action action);

        /// <summary>
        /// Execute the specified <see cref="Action"/> asynchronously 
        /// in the current <see cref="IContext"/>, and return immediately.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        Task BeginInvoke(Action action);
    }
}
