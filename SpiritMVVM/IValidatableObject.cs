using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritMVVM
{
    /// <summary>
    /// Represents an object which notifies listeners of its current validation state.
    /// </summary>
    public interface IValidatableObject : INotifyDataErrorInfo
    {
        /// <summary>
        /// Get the value indicating whether the current instance is valid
        /// (i.e. contains no validation errors).
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Add an error description, associated with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the invalid property.</param>
        /// <param name="error">The error to add.</param>
        void AddError(string propertyName, object error);

        /// <summary>
        /// Clear any errors from the given property name.
        /// </summary>
        /// <param name="propertyName">The property for which to clear any errors.</param>
        void ClearErrors(string propertyName);
    }
}
