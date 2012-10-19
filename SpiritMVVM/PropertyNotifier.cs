using SpiritMVVM.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SpiritMVVM
{
    /// <summary>
    /// Utility class used as a proxy when assigning values to properties,
    /// in order to automatically raise property-changed notification events.
    /// </summary>
    public class PropertyNotifier : IPropertyNotifier
    {
        private readonly Action<string> _raisePropertyChangedAction = null;

        /// <summary>
        /// Creates a new <see cref="PropertyNotifier"/> using the given
        /// <see cref="Action{T}"/> as a property notification delegate.
        /// </summary>
        /// <param name="propertyChangedAction">The Action to execute
        /// whenever any call to the SetProperty method results in a new
        /// value being assigned to a property.</param>
        public PropertyNotifier(Action<string> propertyChangedAction)
        {
            if (propertyChangedAction == null)
                throw new ArgumentNullException("raisePropertyChangedAction");

            _raisePropertyChangedAction = propertyChangedAction;
        }

        /// <summary>
        /// Compare the values of the given backingStore and newValue, assigning
        /// newValue to the backingStore if they are different.  If a new value
        /// was assigned, this method executes the Property-Change notification delegate.
        /// If a new value was assigned an a callback was provided in the
        /// onChangedCallback argument, the callback will be executed,
        /// providing the old value and new values to the handler, respectively.
        /// </summary>
        /// <typeparam name="T">The type of the property being set.</typeparam>
        /// <param name="backingStore">A reference to the backing store for the property.</param>
        /// <param name="newValue">The new value to assign the property, if different.</param>
        /// <param name="onChangedCallback">The callback to execute (aside from the property-
        /// change notification delegate) if the new value is assigned, providing the
        /// old value and new value as arguments, in that order.</param>
        /// <param name="propertyName">The name of the property being changed.  Defaults
        /// to the name of the calling member.  Leave this argument blank if called 
        /// from within the property's "Set" method, and the compiler will automatically
        /// pass the correct property name.</param>
        public void SetProperty<T>(ref T backingStore, T newValue, Action<T, T> onChangedCallback = null, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(backingStore, newValue))
            {
                //Store the old value for the callback
                T oldValue = backingStore;
                backingStore = newValue;

                //Execute the callback, if one was provided
                if (onChangedCallback != null)
                    onChangedCallback(oldValue, newValue);

                //Raise the "PropertyChanged" event
                RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Compare the values of the given backingStore and newValue, assigning
        /// newValue to the backingStore if they are different.  If a new value
        /// was assigned, this method executes the Property-Change notification delegate.
        /// If a new value was assigned an a callback was provided in the
        /// onChangedCallback argument, the callback will be executed,
        /// providing the old value and new values to the handler, respectively.
        /// </summary>
        /// <typeparam name="T">The type of the property being set.</typeparam>
        /// <param name="backingStore">A set of Get/Set accessors for the property's backing store.</param>
        /// <param name="newValue">The new value to assign the property, if different.</param>
        /// <param name="onChangedCallback">The callback to execute (aside from the property-
        /// change notification delegate) if the new value is assigned, providing the
        /// old value and new value as arguments, in that order.</param>
        /// <param name="propertyName">The name of the property being changed.  Defaults
        /// to the name of the calling member.  Leave this argument blank if called 
        /// from within the property's "Set" method, and the compiler will automatically
        /// pass the correct property name.</param>
        public void SetProperty<T>(Accessor<T> backingStore, T newValue, Action<T, T> onChangedCallback = null, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(backingStore.Value, newValue))
            {
                //Store the old value for the callback
                T oldValue = backingStore.Value;
                backingStore.Value = newValue;

                //Execute the callback, if one was provided
                if (onChangedCallback != null)
                    onChangedCallback(oldValue, newValue);

                //Raise the "PropertyChanged" event
                RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Manually raise the internally-stored PropertyChanged notification delegate.
        /// </summary>
        /// <param name="propertyName">The name of the property to provide
        /// the notification delegate.</param>
        public void RaisePropertyChanged(string propertyName)
        {
            var handler = _raisePropertyChangedAction;
            if (handler != null)
            {
                handler(propertyName);
            }
        }
    }
}
