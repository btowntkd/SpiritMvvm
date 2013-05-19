using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using SpiritMVVM.Utils;

namespace SpiritMVVM
{
    /// <summary>
    /// Represents an ObservableObject which has additional validation capabilities.
    /// </summary>
    public class ValidatableObject : ObservableObject, IValidatableObject
    {
        #region Private Fields

        private readonly object _propertyErrorsAccessLock = new object();
        private Dictionary<string, List<object>> _propertyErrorListPairs = new Dictionary<string, List<object>>();

        #endregion

        #region IValidatableObject Implementation

        /// <summary>
        /// Get the value indicating whether the current instance is valid
        /// (i.e. contains no validation errors).
        /// </summary>
        public bool IsValid
        {
            get { return !HasErrors; }
        }

        /// <summary>
        /// Add an error description, associated with the given property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the target property.</typeparam>
        /// <param name="propertyExpression">An expression referencing the target property.</param>
        /// <param name="error">The error to add.</param>
        public void AddError<TProperty>(Expression<Func<TProperty>> propertyExpression, object error)
        {
            AddError(propertyExpression.PropertyName(), error);
        }

        /// <summary>
        /// Add an error description, associated with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the invalid property.</param>
        /// <param name="error">The error to add.</param>
        public void AddError(string propertyName, object error)
        {
            lock (_propertyErrorsAccessLock)
            {
                var currentErrors = GetOrCreateErrorListForProperty(propertyName);
                currentErrors.Add(error);
            }
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Add multiple error descriptions, associated with the given property.
        /// </summary>
        /// <typeparam name="TProperty">The type of the target property.</typeparam>
        /// <param name="propertyExpression">An expression referencing the target property.</param>
        /// <param name="errors">The errors to add.</param>
        public void AddErrors<TProperty>(Expression<Func<TProperty>> propertyExpression, IEnumerable<object> errors)
        {
            AddErrors(propertyExpression.PropertyName(), errors);
        }

        /// <summary>
        /// Add multiple error descriptions, associated with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the invalid property.</param>
        /// <param name="errors">The errors to add.</param>
        public void AddErrors(string propertyName, IEnumerable<object> errors)
        {
            lock (_propertyErrorsAccessLock)
            {
                var currentErrors = GetOrCreateErrorListForProperty(propertyName);
                currentErrors.AddRange(errors);
            }
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clear any errors from the given property name.
        /// </summary>
        /// <param name="propertyName">The property for which to clear any errors.</param>
        public void ClearErrors(string propertyName)
        {
            bool notificationRequired = false;
            lock (_propertyErrorsAccessLock)
            {
                var errors = GetOrCreateErrorListForProperty(propertyName);
                if (errors.Count > 0)
                {
                    errors.Clear();
                    notificationRequired = true;
                }
            }

            //Delay the ErrorsChanged event until we've released the lock
            if (notificationRequired)
            {
                RaiseErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Clear all errors from the current object instance.
        /// </summary>
        public void ClearAllErrors()
        {
            bool notificationRequired = false;
            lock (_propertyErrorsAccessLock)
            {
                if (HasErrors)
                {
                    _propertyErrorListPairs.Clear();
                    notificationRequired = true;
                }
            }

            //Delay the ErrorsChanged event until we've released the lock
            if (notificationRequired)
            {
                RaiseErrorsChanged(string.Empty);
            }
        }

        #endregion

        #region INotifyDataErrorInfo Implementation

        /// <summary>
        /// Event which is raised whenever the validation state of the instance changes.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = null;

        /// <summary>
        /// Get the collection of errors for the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to collect all known errors.</param>
        /// <returns>Returns an enumerable list of 'error' instances.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            IEnumerable errors;
            lock (_propertyErrorsAccessLock)
            {
                //Create a copy of the errors list
                errors = GetOrCreateErrorListForProperty(propertyName)
                        .ToList();
            }
            return errors;
        }

        /// <summary>
        /// Get the value indicating whether nay errors exist on the current instance.
        /// </summary>
        public bool HasErrors
        {
            get { return (GetFlattenedListOfAllErrors().Count > 0); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raise the <see cref="ErrorsChanged"/> event with the given property name as an argument.
        /// </summary>
        /// <param name="propertyName">The name of the property which changed.</param>
        protected virtual void RaiseErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if(handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Query the backing store for a list of errors for the given property.
        /// If no List currently exists; create a new, empty list for that property
        /// and return it.
        /// </summary>
        /// <param name="propertyName">The property key.</param>
        /// <returns>Returns the list of associated errors.</returns>
        private List<object> GetOrCreateErrorListForProperty(string propertyName)
        {
            List<object> errors;
            lock (_propertyErrorsAccessLock)
            {
                if (!_propertyErrorListPairs.TryGetValue(propertyName, out errors))
                {
                    errors = new List<object>();
                    _propertyErrorListPairs[propertyName] = errors;
                }
            }
            return errors;
        }

        /// <summary>
        /// Retrieve a flattened list of all Errors, regardless of property name.
        /// </summary>
        /// <returns>Returns the flattened list of all validation errors.</returns>
        private List<object> GetFlattenedListOfAllErrors()
        {
            List<object> flattenedErrors;
            lock (_propertyErrorsAccessLock)
            {
                flattenedErrors = _propertyErrorListPairs.SelectMany(p => p.Value).ToList();
            }

            return flattenedErrors;
        }

        #endregion
    }
}
