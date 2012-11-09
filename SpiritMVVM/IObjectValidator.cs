
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
        /// Validate the given property name of the object.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        void Validate(TParent instance, string propertyName);
    }
}
