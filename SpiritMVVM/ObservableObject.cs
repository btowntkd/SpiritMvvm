using SpiritMVVM.PropertyMapping;
using SpiritMVVM.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SpiritMVVM
{
    /// <summary>
    /// Represents a base class implementing the <see cref="INotifyPropertyChanged"/> interface.
    /// Contains helper methods for Setting a new value to a property and automatically raising
    /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the property,
    /// along with all dependents.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged, IMapDependencies
    {
        #region Private Fields

        private Dictionary<string, List<string>> _propertyDependencies = new Dictionary<string, List<string>>();
        private object _propertyDependenciesLock = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Create a new instance of an <see cref="ObservableObject"/>.
        /// </summary>
        public ObservableObject()
        {
            ScanForDependsOnAttributes();
        }

        #endregion Constructors

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
            SetPropertyAndRaiseEvents(targetProperty.PropertyName(), ref backingStore, newValue, onChangedCallback);
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
            SetPropertyAndRaiseEvents(targetProperty.PropertyName(), backingStore, newValue, onChangedCallback);
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
            SetPropertyAndRaiseEvents(targetPropertyName, ref backingStore, newValue, onChangedCallback);
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
            SetPropertyAndRaiseEvents(targetPropertyName, backingStore, newValue, onChangedCallback);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Compare the values of the given backingStore and newValue, assigning
        /// newValue to the backingStore if they are different.  If a new value
        /// was assigned, this method first executes the onChangedCallback delegate,
        /// providing the old value and new values to the handler, respectively.
        /// This method will then proceeds to raise the PropertyChanged event for that property.
        /// </summary>
        /// <typeparam name="T">The type of the property being set.</typeparam>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="backingStore">A reference to the backing store for the property.</param>
        /// <param name="newValue">The new value to assign the property, if different.</param>
        /// <param name="onChangedCallback">The callback to execute (aside from the property-
        /// change notification delegate) if the new value is assigned, providing the
        /// old value and new value as arguments, in that order.</param>
        private void SetPropertyAndRaiseEvents<T>(string propertyName, ref T backingStore, T newValue, Action<T, T> onChangedCallback = null)
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
        /// was assigned, this method first executes the onChangedCallback delegate,
        /// providing the old value and new values to the handler, respectively.
        /// This method will then proceeds to raise the PropertyChanged event for that property.
        /// </summary>
        /// <typeparam name="T">The type of the property being set.</typeparam>
        /// <param name="propertyName">The name of the property being changed.</param>
        /// <param name="backingStore">A set of Get/Set accessors for the property's backing store.</param>
        /// <param name="newValue">The new value to assign the property, if different.</param>
        /// <param name="onChangedCallback">The callback to execute (aside from the property-
        /// change notification delegate) if the new value is assigned, providing the
        /// old value and new value as arguments, in that order.</param>
        private void SetPropertyAndRaiseEvents<T>(string propertyName, Accessor<T> backingStore, T newValue, Action<T, T> onChangedCallback = null)
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
        /// Raises the "PropertyChanged" event for a given property's dependents.
        /// The event is not raised for the given property, itself.
        /// </summary>
        /// <param name="propertyName">The name of the property whose dependents
        /// should be raised.</param>
        /// <remarks>Dependant properties are any properties marked with the
        /// <see cref="DependsOnAttribute"/> with the given property named as
        /// the dependency.</remarks>
        private void RaisePropertyChangedDependants(string propertyName)
        {
            var dependants = GetDependentsFor(propertyName);
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

        #endregion INotifyPropertyChanged

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
        /// <remarks>The resulting list should include indirect dependents
        /// as well as direct ones (i.e. if A depends on B, and B depends on C,
        /// then the list of dependents from C will include both A and B).</remarks>
        /// <param name="propertyName">The property for which to gather all dependents.</param>
        /// <returns>Returns the list of all properties which are dependent on the given property.</returns>
        public IEnumerable<string> GetDependentsFor(string propertyName)
        {
            IEnumerable<string> oldResults = null;
            IEnumerable<string> results = new[] { propertyName };
            do
            {
                oldResults = results;

                var groupDependents = from input in results
                                      from dependent in GetDirectDependentsFor(input)
                                      select dependent;

                //Create union of current results with "new" results,
                //making sure to remove duplicates
                results = results.Union(groupDependents)
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
        /// <remarks>The resulting list should include indirect dependents
        /// as well as direct ones (i.e. if A depends on B, and B depends on C,
        /// then the list of dependents from C will include both A and B).</remarks>
        /// <typeparam name="TProperty">The type of the target property.</typeparam>
        /// <param name="propertyExpression">The property for which to gather all dependents.</param>
        /// <returns>Returns the list of all properties which are dependent on the given property.</returns>
        public IEnumerable<string> GetDependentsFor<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            return GetDependentsFor(propertyExpression.PropertyName());
        }

        /// <summary>
        /// Get the direct dependents for a given property.
        /// </summary>
        /// <param name="propertyName">The property for which to collect all direct dependents.</param>
        /// <returns>Returns a list of all properties which directly depend on the given property.</returns>
        private IEnumerable<string> GetDirectDependentsFor(string propertyName)
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

                foreach (var dependency in dependencies)
                {
                    AddPropertyDependency(property.Name, dependency);
                }
            }
        }

        /// <summary>
        /// Adds a property dependency mapping to the map.
        /// When adding a property dependency, the parameters construct the sentence:
        /// "Argument 1 depends on Argument2."
        /// </summary>
        /// <param name="dependantPropertyName">The target (dependent) property.</param>
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

        #endregion IMapDependencies
    }
}