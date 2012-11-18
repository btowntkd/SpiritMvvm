﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        /// Add an error description, associated with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the invalid property.</param>
        /// <param name="error">The error to add.</param>
        public void AddError(string propertyName, object error)
        {
            lock (_propertyErrorsAccessLock)
            {
                var errors = GetOrCreateErrorListForProperty(propertyName);
                errors.Add(error);
            }
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clear any errors from the given property name.
        /// </summary>
        /// <param name="propertyName">The property for which to clear any errors.</param>
        public void ClearErrors(string propertyName)
        {
            bool errorsCleared = false;
            lock (_propertyErrorsAccessLock)
            {
                var errors = GetOrCreateErrorListForProperty(propertyName);
                if (errors.Count > 0)
                {
                    errors.Clear();
                    errorsCleared = true;
                }
            }

            //Delay the ErrorsChanged event until we've released the lock
            if (errorsCleared)
            {
                RaiseErrorsChanged(propertyName);
            }
        }

        #endregion

        #region INotifyDataErrorInfo Implementation

        /// <summary>
        /// Event which is raised whenever the validation state of the instance changes.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = null;

        /// <summary>
        /// Get the value indicating whether nay errors exist on the current instance.
        /// </summary>
        public bool HasErrors
        {
            get { return (GetFlattenedListOfAllErrors().Count > 0); }
        }

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
            if (!_propertyErrorListPairs.TryGetValue(propertyName, out errors))
            {
                errors = new List<object>();
                _propertyErrorListPairs[propertyName] = errors;
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
