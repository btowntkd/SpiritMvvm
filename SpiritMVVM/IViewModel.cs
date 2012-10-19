using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using SpiritMVVM.Messaging;

namespace SpiritMVVM
{
    /// <summary>
    /// Provides common ViewModel-related functionality.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Get or Set a messenger object used for loosely-coupled messaging.
        /// </summary>
        IMessenger Messenger { get; set; }
    }

    /// <summary>
    /// Provides common ViewModel-related functionality, as well as
    /// exposing an underlying Model for which the ViewModel wraps.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IViewModel<TModel> : IViewModel
    {
        /// <summary>
        /// Get or Set the underlying Model which the ViewModel wraps.
        /// </summary>
        TModel Model { get; set; }
    }
}
