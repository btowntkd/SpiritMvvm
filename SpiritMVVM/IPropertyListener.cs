﻿using System;

namespace SpiritMVVM
{
    /// <summary>
    /// Interface for subscribing to specific property change notifications.
    /// </summary>
    public interface IPropertyListener
    {
        /// <summary>
        /// Registers an action to be executed any time the given property
        /// is changed.
        /// </summary>
        /// <typeparam name="T">The target property's type.</typeparam>
        /// <param name="propertyName">The name of the property to monitor
        /// for changes.</param>
        /// <param name="action">The action to execute whenever the property
        /// changes, providing the new property value.</param>
        void AddListener<T>(string propertyName, Action<T> action);

        /// <summary>
        /// Remove all registered listeners for the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property for which
        /// to remove all listeners.</param>
        void RemoveListeners(string propertyName);
    }
}