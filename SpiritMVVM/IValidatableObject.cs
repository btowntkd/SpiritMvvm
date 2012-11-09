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
        bool IsValid { get; }

        void AddError(string propertyName, string error);
        void ClearErrors(string propertyName);
    }
}
