using System.Threading.Tasks;

namespace SpiritMVVM
{
    /// <summary>
    /// An interface used for validating a parent class instance.
    /// </summary>
    /// <typeparam name="TParent">The parent class which the interface will validate.</typeparam>
    public interface IObjectValidator<TParent>
        where TParent : IValidatableObject
    {
        /// <summary>
        /// Validate the entire object instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        void Validate(TParent instance);

        /// <summary>
        /// Validate the specified property of the given object instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        void ValidateProperty(TParent instance, string propertyName);

        /// <summary>
        /// Validate the entire object instance asynchronously.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>Returns the asynchronous Validation task.</returns>
        Task ValidateAsync(TParent instance);

        /// <summary>
        /// Validate the specified property of the given object instance asynchronously.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>Returns the asynchronous Validation task.</returns>
        Task ValidatePropertyAsync(TParent instance, string propertyName);
    }
}