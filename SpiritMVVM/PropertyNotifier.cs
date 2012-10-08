using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMVVM
{
    /// <summary>
    /// Utility class used as a proxy when assigning values to properties,
    /// in order to automatically raise property-change notification events.
    /// </summary>
    public class PropertyNotifier
    {
        private Action<string> _propertyNotificationAction = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyChangedAction"></param>
        public PropertyNotifier(Action<string> propertyChangedAction)
        {
            if (propertyChangedAction == null)
                throw new ArgumentNullException("propertyChangedAction");

            _propertyNotificationAction = propertyChangedAction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="newValue"></param>
        /// <param name="onChangedCallback"></param>
        /// <param name="propertyName"></param>
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

                //Raise the notification event.
                RaiseNotification(propertyName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="newValue"></param>
        /// <param name="onChangedCallback"></param>
        /// <param name="propertyName"></param>
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

                //Raise the notification event.
                RaiseNotification(propertyName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public void RaiseNotification(string propertyName)
        {
            var handler = _propertyNotificationAction;
            if (handler != null)
            {
                handler(propertyName);
            }
        }
    }
}
