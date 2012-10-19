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
    /// 
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        #region Private Fields

        private IPropertyNotifier _propertyNotifier;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ObservableObject()
        {
            PropertyNotifier = new PropertyNotifier((propName) => 
            {
                RaisePropertyChanged(propName);
                RaisePropertyChangedDependants(propName);
            });
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Get or Set the internally-stored <see cref="IPropertyNotifier"/> object
        /// which is used for assisting with setting property values via the 
        /// ObservableObject.Set methods.
        /// </summary>
        protected IPropertyNotifier PropertyNotifier
        {
            get { return _propertyNotifier; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "PropertyHelper cannot be null");

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
        /// 
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
            var dependants = DependsOnAttribute.GetAllDependants(this.GetType(), propertyName);
            foreach (var dependant in dependants)
            {
                RaisePropertyChanged(dependant.Name);
            }
        }

        #endregion
    }
}
