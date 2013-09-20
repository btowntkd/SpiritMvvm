namespace SpiritMVVM
{
    /// <summary>
    /// Describes an object which can exist as a property within an
    /// <see cref="ObservableObject"/>, and can react with a custom action when
    /// a property-change dependency changes.
    /// </summary>
    public interface IReactOnDependencyChanged
    {
        /// <summary>
        /// Executes the custom action when the dependency is changed.
        /// </summary>
        void OnDependencyChanged();
    }
}