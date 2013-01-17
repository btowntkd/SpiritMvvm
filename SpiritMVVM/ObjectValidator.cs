using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;

namespace SpiritMVVM
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    public class ObjectValidator<TParent> : IObjectValidator<TParent>
        where TParent : IValidatableObject
    {
        /// <summary>
        /// Validate the entire object instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        public void Validate(TParent instance)
        {
            var validationContext = new ValidationContext(instance);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(instance, validationContext, validationResults, true);

            var resultsByProperty = from result in validationResults
                                    from member in result.MemberNames
                                    group result by member into resultGroups
                                    select resultGroups;

            instance.ClearAllErrors();
            foreach (var prop in resultsByProperty)
            {
                var messages = prop.Select(r => r.ErrorMessage);
                instance.AddErrors(prop.Key, messages);
            }
        }

        /// <summary>
        /// Validate the specified property of the given object instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        public void ValidateProperty(TParent instance, string propertyName)
        {
            var validationContext = new ValidationContext(instance);
            validationContext.MemberName = propertyName;
            var validationResults = new List<ValidationResult>();
            object propertyValue = instance.GetType()
                .GetRuntimeProperty(propertyName)
                .GetValue(instance);

            Validator.TryValidateProperty(propertyValue, validationContext, validationResults);
            instance.ClearErrors(propertyName);
            var errorMessages = validationResults.Select(r => r.ErrorMessage);
            instance.AddErrors(propertyName, errorMessages);
        }

        /// <summary>
        /// Validate the entire object instance asynchronously.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>Returns the asynchronous Validation task.</returns>
        public Task ValidateAsync(TParent instance)
        {
            return Task.Run(() => Validate(instance));
        }

        /// <summary>
        /// Validate the specified property of the given object instance asynchronously.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>Returns the asynchronous Validation task.</returns>
        public Task ValidatePropertyAsync(TParent instance, string propertyName)
        {
            return Task.Run(() => ValidateProperty(instance, propertyName));
        }
    }
}
