﻿using System.ComponentModel;
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
}
