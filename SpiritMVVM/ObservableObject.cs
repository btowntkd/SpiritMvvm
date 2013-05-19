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
        private Dictionary<string, List<string>> _propertyDependencies = new Dictionary<string, List<string>>();
        private object _propertyDependenciesLock = new object();

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
                RaisePropertyChanged(propName);
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
        /// <typeparam name="TProperty">The type of the value being set.</typeparam>
        /// <param name="targetProperty">An expression referencing the property being set.</param>
        /// <param name="backingStore">The backing field for the property.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <param name="onChangedCallback">The optional callback to execute if the value changed.</param>
        protected void Set<TProperty>(Expression<Func<TProperty>> targetProperty, ref TProperty backingStore, TProperty newValue, Action<TProperty, TProperty> onChangedCallback = null)
        {
            PropertyNotifier.SetProperty(ref backingStore, newValue, onChangedCallback, targetProperty.PropertyName());
        }

        /// <summary>
        /// Assign the given value to the ref backing store field.  If the value changed,
        /// the method will raise the <see cref="ObservableObject.PropertyChanged"/> event,
        /// and will execute the optional provided callback - passing along the old and new values,
        /// respectively.
        /// </summary>
        /// <typeparam name="TProperty">The type of the value being set.</typeparam>
        /// <param name="targetProperty">An expression referencing the property being set.</param>
        /// <param name="backingStore">The backing field for the property.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <param name="onChangedCallback">The optional callback to execute if the value changed.</param>
        protected void Set<TProperty>(Expression<Func<TProperty>> targetProperty, Accessor<TProperty> backingStore, TProperty newValue, Action<TProperty, TProperty> onChangedCallback = null)
        {
            PropertyNotifier.SetProperty(backingStore, newValue, onChangedCallback, targetProperty.PropertyName());
        }

        /// <summary>
        /// Assign the given value to the ref backing store field.  If the value changed,
        /// the method will raise the <see cref="ObservableObject.PropertyChanged"/> event,
        /// and will execute the optional provided callback - passing along the old and new values,
        /// respectively.
        /// </summary>
        /// <typeparam name="TProperty">The type of the value being set.</typeparam>
        /// <param name="backingStore">The backing field for the property.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <param name="onChangedCallback">The optional callback to execute if the value changed.</param>
        /// <param name="targetPropertyName">
        /// The name of the property being set.
        /// Uses <see cref="CallerMemberNameAttribute"/> to automatically populate the value with the caller's name.
        /// </param>
        protected void Set<TProperty>(ref TProperty backingStore, TProperty newValue, Action<TProperty, TProperty> onChangedCallback = null, [CallerMemberName] string targetPropertyName = "")
        {
            PropertyNotifier.SetProperty(ref backingStore, newValue, onChangedCallback, targetPropertyName);
        }

        /// <summary>
        /// Assign the given value to the ref backing store field.  If the value changed,
        /// the method will raise the <see cref="ObservableObject.PropertyChanged"/> event,
        /// and will execute the optional provided callback - passing along the old and new values,
        /// respectively.
        /// </summary>
        /// <typeparam name="TProperty">The type of the value being set.</typeparam>
        /// <param name="backingStore">The backing field for the property.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <param name="onChangedCallback">The optional callback to execute if the value changed.</param>
        /// <param name="targetPropertyName">
        /// The name of the property being set.
        /// Uses <see cref="CallerMemberNameAttribute"/> to automatically populate the value with the caller's name.
        /// </param>
        protected void Set<TProperty>(Accessor<TProperty> backingStore, TProperty newValue, Action<TProperty, TProperty> onChangedCallback = null, [CallerMemberName] string targetPropertyName = "")
        {
            PropertyNotifier.SetProperty(backingStore, newValue, onChangedCallback, targetPropertyName);
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
        /// <param name="propertyExpression">The property which changed.</param>
        protected virtual void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            RaisePropertyChanged(propertyExpression.PropertyName());
        }

        /// <summary>
        /// Raises the "PropertyChanged" event with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property which changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            RaisePropertyChangedEvent(propertyName);
            RaisePropertyChangedDependants(propertyName);
        }

        /// <summary>
        /// Raises a single "PropertyChanged" event with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property which changed.</param>
        private void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the "PropertyChanged" event for a given property's dependants.
        /// The event is not raised for the given property, itself.
        /// </summary>
        /// <param name="propertyName">The name of the property whose dependants
        /// should be raised.</param>
        /// <remarks>Dependant properties are any properties marked with the 
        /// <see cref="DependsOnAttribute"/> with the given property named as
        /// the dependency.</remarks>
        private void RaisePropertyChangedDependants(string propertyName)
        {
            var dependants = GetDependantsFor(propertyName);
            foreach (var dependant in dependants)
            {
                object dependantValue = this.GetType().GetRuntimeProperty(dependant).GetValue(this);
                if (dependantValue is IReactOnDependencyChanged)
                {
                    ((IReactOnDependencyChanged)dependantValue).OnDependencyChanged();
                }

                RaisePropertyChangedEvent(dependant);
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
                AddPropertyDependency(propertyName, dependency);
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
            IEnumerable<string> oldResults = null;
            IEnumerable<string> results = new[] { propertyName };
            do
            {
                oldResults = results;

                var groupDependants = from input in results
                                      from dependancy in GetDirectDependantsFor(input)
                                      select dependancy;

                //Create union of current results with "new" results,
                //making sure to remove duplicates
                results = results.Union(groupDependants)
                    .GroupBy(x => x)
                    .Select(grp => grp.First());
            }
            while (results.Count() > oldResults.Count());

            //Return results not including the original property name
            return results.Where(x => (x != propertyName));
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
        /// Get the direct dependants for a given property.
        /// </summary>
        /// <param name="propertyName">The property for which to collect all direct dependants.</param>
        /// <returns>Returns a list of all properties which directly depend on the given property.</returns>
        private IEnumerable<string> GetDirectDependantsFor(string propertyName)
        {
            lock (_propertyDependenciesLock)
            {
                return from dependenciesKVP in _propertyDependencies
                       where dependenciesKVP.Key != propertyName //Ignore the original property, if it depends on itself
                       where dependenciesKVP.Value.Any(dependency => (dependency == propertyName))
                       select dependenciesKVP.Key;
            }
        }

        /// <summary>
        /// Retrieve the list of dependencies for the current property, from the backing store.
        /// If no list currently exists, create one and add it to the backing store automatically,
        /// before returning.
        /// </summary>
        /// <param name="propertyKey">The property for which to retrieve the list of dependencies.</param>
        /// <returns>Returns the current list of dependencies for the given property, for editing.</returns>
        private List<string> GetOrCreateDependenciesList(string propertyKey)
        {
            List<string> result = null;
            lock (_propertyDependenciesLock)
            {
                if (!_propertyDependencies.TryGetValue(propertyKey, out result))
                {
                    result = new List<string>();
                    _propertyDependencies[propertyKey] = result;
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
                var dependencies = property.GetCustomAttributes(typeof(DependsOnAttribute), true)
                    .Cast<DependsOnAttribute>()
                    .Select(attribute => attribute.Property);

                foreach(var dependency in dependencies)
                {
                    AddPropertyDependency(property.Name, dependency);
                }
            }
        }

        /// <summary>
        /// Adds a property dependency mapping to the map.
        /// </summary>
        /// <param name="dependantPropertyName">The target (dependant) property.</param>
        /// <param name="dependencyPropertyName">The property on which the target property depends.</param>
        private void AddPropertyDependency(string dependantPropertyName, string dependencyPropertyName)
        {
            lock (_propertyDependenciesLock)
            {
                var dependenciesList = GetOrCreateDependenciesList(dependantPropertyName);
                if (!dependenciesList.Contains(dependencyPropertyName))
                {
                    dependenciesList.Add(dependencyPropertyName);
                }
            }
        }

        #endregion
    }
}
