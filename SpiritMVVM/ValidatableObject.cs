using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMVVM
{
    /// <summary>
    /// Represents an ObservableObject which has additional validation capabilities.
    /// </summary>
    public class ValidatableObject : ObservableObject, IValidatableObject
    {
        private Dictionary<string, List<object>> _propertyErrors = new Dictionary<string, List<object>>();

        /// <summary>
        /// Event which is raised whenever the validation state of the instance changes.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = null;

        /// <summary>
        /// Get the value indicating whether the current instance is valid
        /// (i.e. contains no validation errors).
        /// </summary>
        public bool IsValid
        {
            get { return !HasErrors; }
        }

        /// <summary>
        /// Get the value indicating whether nay errors exist on the current instance.
        /// </summary>
        public bool HasErrors
        {
            get { return _propertyErrors.Values.SelectMany((x) => x).Count() > 0; }
        }

        /// <summary>
        /// Add an error description, associated with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the invalid property.</param>
        /// <param name="error">The error to add.</param>
        public void AddError(string propertyName, object error)
        {
            List<object> currentErrors;
            if (!_propertyErrors.TryGetValue(propertyName, out currentErrors))
            {
                currentErrors = new List<object>();
                _propertyErrors[propertyName] = currentErrors;
            }

            currentErrors.Add(error);
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clear any errors from the given property name.
        /// </summary>
        /// <param name="propertyName">The property for which to clear any errors.</param>
        public void ClearErrors(string propertyName)
        {
            if (_propertyErrors.ContainsKey(propertyName))
            {
                _propertyErrors[propertyName].Clear();
            }
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            List<object> errors;
            if (!_propertyErrors.TryGetValue(propertyName, out errors))
            {
                errors = new List<object>();
            }

            return errors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaiseErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if(handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}
