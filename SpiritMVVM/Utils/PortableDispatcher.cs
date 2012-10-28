using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiritMVVM.Utils
{
    /// <summary>
    /// Class used for dispatching methods to a specific <see cref="SynchronizationContext"/>.
    /// </summary>
    public class PortableDispatcher
    {
        private SynchronizationContext _context;

        /// <summary>
        /// Get or Set the SynchronizationContext associated with the current dispatcher instance.
        /// </summary>
        public SynchronizationContext Context
        {
            get { return _context; }
            set
            {
                _context = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public void BeginInvoke(Action action)
        {
            Context.Post((x) => action(), null);
        }

    }
}
