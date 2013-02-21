using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using SpiritMVVM.PropertyMapping;
using SpiritMVVM.Utils;

namespace SpiritMVVM
{
    /// <summary>
    /// Represents a base class implementing the <see cref="INotifyPropertyChanged"/> interface
    /// through the use of an <see cref="IPropertyNotifier"/> utility instance.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged, IMapDependencies
    {
        #region Private Fields

        private IPropertyNotifier _propertyNotifier;
        private Dictionary<string, List<string>> _propertyDependants = new Dictionary<string, List<string>>();
        private object _propertyDependantsLock = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of an <see cref="ObservableObject"/>, with the default
        /// <see cref="PropertyNotifier"/> implementation raising an event
        /// for the changed property and all dependant properties.
        /// </summary>
        public ObservableObject()
        {
            PropertyNotifier = new PropertyNotifier((propName) => 
            {
                this.RaisePropertyChanged(propName);
                this.RaisePropertyChangedDependants(propName);
            });
            ScanForDependsOnAttributes();
        }

        /// <summary>
        /// Create a new instance of an <see cref="ObservableObject"/>,
        /// with a user-specified <see cref="IPropertyNotifier"/> implementation.
        /// </summary>
        /// <param name="propertyNotifier">The user-specified 
        /// <see cref="IPropertyNotifier"/> implementation.</param>
        public ObservableObject(IPropertyNotifier propertyNotifier)
        {
            if (propertyNotifier == null)
                throw new ArgumentNullException("propertyNotifier");

            PropertyNotifier = propertyNotifier;
            ScanForDependsOnAttributes();
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Get or Set the internally-used <see cref="IPropertyNotifier"/> object.
        /// This object is used when setting property values via the 
        /// ObservableObject.Set methods.
        /// </summary>
        protected IPropertyNotifier PropertyNotifier
        {
            get { return _propertyNotifier; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "PropertyNotifier cannot be null");

                _propertyNotifier = value;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Assign the given value to the ref backing store field.  If the value changed,
        /// the method will raise the <see cref="ObservableObject.PropertyChanged"/> event,
        /// and will execute the optional provided callback - passing along the old and new values,
        /// respectively.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="backingStore">The backing field for the property.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <param name="onChangedCallback">The optional callback to execute if the value changed.</param>
        /// <param name="propertyName">The name of the property being set.</param>
        protected void Set<T>(ref T backingStore, T newValue, Action<T, T> onChangedCallback = null, [CallerMemberName] string propertyName = "")
        {
            PropertyNotifier.SetProperty(ref backingStore, newValue, onChangedCallback, propertyName);
        }

        /// <summary>
        /// Assign the given value to the ref backing store field.  If the value changed,
        /// the method will raise the <see cref="ObservableObject.PropertyChanged"/> event,
        /// and will execute the optional provided callback - passing along the old and new values,
        /// respectively.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="backingStore">The backing field for the property.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <param name="onChangedCallback">The optional callback to execute if the value changed.</param>
        /// <param name="propertyName">The name of the property being set.</param>
        protected void Set<T>(Accessor<T> backingStore, T newValue, Action<T, T> onChangedCallback = null, [CallerMemberName] string propertyName = "")
        {
            PropertyNotifier.SetProperty(backingStore, newValue, onChangedCallback, propertyName);
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Event raised whenever a member property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = null;

        /// <summary>
        /// Raises the "PropertyChanged" event with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property which changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the "PropertyChanged" event a given property's dependants.
        /// The event is not raised for the given property, itself.
        /// </summary>
        /// <param name="propertyName">The name of the property whose dependants
        /// should be raised.</param>
        /// <remarks>Dependant properties are any properties marked with the 
        /// <see cref="DependsOnAttribute"/> with the given property named as
        /// the dependency.</remarks>
        protected virtual void RaisePropertyChangedDependants(string propertyName)
        {
            var dependants = GetDependantsFor(propertyName);
            foreach (var dependant in dependants)
            {
                object dependantValue = this.GetType().GetRuntimeProperty(dependant).GetValue(this);
                if (dependantValue is IReactOnDependencyChanged)
                {
                    ((IReactOnDependencyChanged)dependantValue).OnDependencyChanged();
                }

                RaisePropertyChanged(dependant);
            }
        }

        #endregion

        #region IMapDependencies

        /// <summary>
        /// Begin mapping a new property's dependencies using fluent syntax.
        /// </summary>
        /// <param name="propertyName">The property for which to add dependencies.</param>
        /// <returns>Returns a fluent property dependency builder.</returns>
        public IPropertyMapBuilder Property(string propertyName)
        {
            return new PropertyMapBuilder((dependency) =>
            {
                lock (_propertyDependantsLock)
                {
                    var dependantsList = GetOrCreateDependantsList(dependency);
                    if (!dependantsList.Contains(propertyName))
                    {
                        dependantsList.Add(propertyName);
                    }
                }
            });
        }

        /// <summary>
        /// Begin mapping a new property's dependencies using fluent syntax.
        /// </summary>
        /// <typeparam name="TProperty">The type of the target property.</typeparam>
        /// <param name="propertyExpression">The property for which to add dependencies.</param>
        /// <returns>Returns a fluent property dependency builder.</returns>
        public IPropertyMapBuilder Property<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            return Property(propertyExpression.PropertyName());
        }

        /// <summary>
        /// Get the list of all properties which depend on the given property.
        /// </summary>
        /// <remarks>The resulting list should include indirect dependants
        /// as well as direct ones (i.e. if A depends on B, and B depends on C,
        /// then the list of dependants from C will include both A and B).</remarks>
        /// <param name="propertyName">The property for which to gather all dependants.</param>
        /// <returns>Returns the list of all properties which are dependant on the given property.</returns>
        public IEnumerable<string> GetDependantsFor(string propertyName)
        {
            List<string> dependants;
            lock (_propertyDependantsLock)
            {
                //Create a copy of the dependency list, for thread-safety
                dependants = GetOrCreateDependantsList(propertyName)
                    .ToList();
            }
            return dependants;
        }

        /// <summary>
        /// Get the list of all properties which depend on the given property.
        /// </summary>
        /// <remarks>The resulting list should include indirect dependants
        /// as well as direct ones (i.e. if A depends on B, and B depends on C,
        /// then the list of dependants from C will include both A and B).</remarks>
        /// <typeparam name="TProperty">The type of the target property.</typeparam>
        /// <param name="propertyExpression">The property for which to gather all dependants.</param>
        /// <returns>Returns the list of all properties which are dependant on the given property.</returns>
        public IEnumerable<string> GetDependantsFor<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            return GetDependantsFor(propertyExpression.PropertyName());
        }

        /// <summary>
        /// Retrieve the list of dependencies for the current property, from the backing store.
        /// If no list currently exists, create one and add it to the backing store automatically,
        /// before returning.
        /// </summary>
        /// <param name="propertyKey">The property for which to retrieve the list of dependencies.</param>
        /// <returns>Returns the current list of dependencies for the given property, for editing.</returns>
        private List<string> GetOrCreateDependantsList(string propertyKey)
        {
            List<string> result = null;
            lock (_propertyDependantsLock)
            {
                if (!_propertyDependants.TryGetValue(propertyKey, out result))
                {
                    result = new List<string>();
                    _propertyDependants[propertyKey] = result;
                }
            }
            return result;
        }

        /// <summary>
        /// Scans over the current Type, detecting any usage of the "DependsOn" attribute
        /// and adding it to the list of property dependencies.
        /// </summary>
        private void ScanForDependsOnAttributes()
        {
            Type type = this.GetType();
            var runtimeProperties = type.GetRuntimeProperties();

            foreach (var property in runtimeProperties)
            {
                var attributeDependencies = DependsOnAttribute.GetAllDependants(type, property.Name);
                lock(_propertyDependantsLock)
                {
                    List<string> dependencies = GetOrCreateDependantsList(property.Name);
                    foreach (var dependencyToAdd in attributeDependencies)
                    {
                        if (!dependencies.Contains(dependencyToAdd.Name))
                        {
                            dependencies.Add(dependencyToAdd.Name);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
