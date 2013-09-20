using SpiritMVVM.Utils;
using System;

namespace SpiritMVVM
{
    /// <summary>
    /// Common interface for proxy classes which assist in assigning new values
    /// to Properties, raising callback methods when the property's value changes.
    /// </summary>
    public interface IPropertyNotifier
    {
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
        /// <param name="onChangedCallback">A callback to execute if the new value is assigned,
        /// providing the old value and new value as arguments, in that order.</param>
        /// <param name="propertyName">The name of the property being changed.</param>
        void SetProperty<T>(ref T backingStore, T newValue, Action<T, T> onChangedCallback, string propertyName);

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
        /// <param name="onChangedCallback">A callback to execute if the new value is assigned,
        /// providing the old value and new value as arguments, in that order.</param>
        /// <param name="propertyName">The name of the property being changed</param>
        void SetProperty<T>(Accessor<T> backingStore, T newValue, Action<T, T> onChangedCallback, string propertyName);
    }
}