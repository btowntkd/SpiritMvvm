using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SpiritMVVM
{
    /// <summary>
    /// A utility class which listens for <see cref="INotifyPropertyChanged.PropertyChanged"/> events
    /// on a target object, and executes a given callback whenever the <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// event is raised for a specific, registered property name.
    /// </summary>
    public class PropertyListener
    {
        private readonly WeakEvent
        private readonly object _listenerLock = new object();
        private readonly Dictionary<string, List<IListenerAction>> _listeners =
            new Dictionary<string,List<IListenerAction>>();


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
            WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs> test;
            _target = new WeakReference(parentObject);
            parentObject.PropertyChanged += new PropertyChangedEventHandler(target_PropertyChanged);
        }

        void target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        internal interface IListenerAction
        {
            void Execute(object);
        }

        internal class ListenerAction<T> : IListenerAction
        {

        }
    }
}
