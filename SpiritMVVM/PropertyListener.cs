using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SpiritMVVM
{
    /// <summary>
    /// A utility class which listens for <see cref="INotifyPropertyChanged.PropertyChanged"/> events
    /// on a target object, and executes a given callback whenever the <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// event is raised for a specific, registered property name.
    /// </summary>
    public class PropertyListener : IPropertyListener
    {
        #region Private Fields

        private readonly WeakReference _weakParentObject;
        private readonly object _listenerLock = new object();
        private readonly Dictionary<string, List<IListenerAction>> _listeners =
            new Dictionary<string,List<IListenerAction>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of the <see cref="PropertyListener"/>
        /// with the given <see cref="INotifyPropertyChanged"/> target object
        /// to listen for changes.
        /// </summary>
        /// <param name="parentObject"></param>
        public PropertyListener(INotifyPropertyChanged parentObject)
        {
            if (parentObject == null)
                throw new ArgumentNullException("parentObject");

            _weakParentObject = new WeakReference(parentObject);
            parentObject.PropertyChanged += new PropertyChangedEventHandler(parentObject_PropertyChanged);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers an action to be executed any time the given property
        /// is changed.
        /// </summary>
        /// <typeparam name="T">The target property's type.</typeparam>
        /// <param name="propertyName">The name of the property to monitor
        /// for changes.</param>
        /// <param name="action">The action to execute whenever the property
        /// changes, providing the new property value.</param>
        public void AddListener<T>(string propertyName, Action<T> action)
        {
            lock (_listenerLock)
            {
                List<IListenerAction> currentListeners = null;
                if (!_listeners.TryGetValue(propertyName, out currentListeners))
                {
                    currentListeners = new List<IListenerAction>();
                    _listeners[propertyName] = currentListeners;
                }

                currentListeners.Add(new ListenerAction<T>(action));
            }
        }

        /// <summary>
        /// Remove all registered listeners for the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property for which
        /// to remove all listeners.</param>
        public void RemoveListeners(string propertyName)
        {
            lock (_listenerLock)
            {
                if (_listeners.ContainsKey(propertyName))
                {
                    _listeners.Remove(propertyName);
                }
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes every subscribed action for the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to execute all subscribed actions.</param>
        protected void ExecuteListenerActions(string propertyName)
        {
            object parentObject = _weakParentObject.Target;
            List<IListenerAction> currentListeners = null;
            lock (_listenerLock)
            {
                if (parentObject != null
                    && _listeners.TryGetValue(propertyName, out currentListeners))
                {
                    try
                    {
                        PropertyInfo propInfo = parentObject.GetType().GetRuntimeProperty(propertyName);
                        object value = propInfo.GetValue(parentObject);
                        foreach (var currentListener in currentListeners)
                        {
                            currentListener.Execute(value);
                        }
                    }
                    catch (Exception)
                    {
                        /* Do nothing: fail silently */
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the parent object's <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The object which raised the event.</param>
        /// <param name="e">The event args.</param>
        private void parentObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ExecuteListenerActions(e.PropertyName);
        }

        #endregion

        internal interface IListenerAction
        {
            void Execute(object parameter);
        }

        internal class ListenerAction<T> : IListenerAction
        {
            private readonly Action<T> _listenerAction;
            public ListenerAction(Action<T> listenerAction)
            {
                _listenerAction = listenerAction;
            }

            public void Execute(object parameter)
            {
                if (_listenerAction != null)
                {
                    _listenerAction((T)parameter);
                }
            }
        }
    }
}
