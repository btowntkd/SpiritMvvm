
namespace SpiritMVVM.ViewModels
{
    /// <summary>
    /// Provides common ViewModel-related functionality, as well as
    /// exposing an underlying Model for which the ViewModel wraps.
    /// </summary>
    /// <typeparam name="TModel">The type of the underlying model.</typeparam>
    public interface IViewModel<TModel> : IViewModel
    {
        /// <summary>
        /// Get or Set the underlying Model which the ViewModel wraps.
        /// </summary>
        TModel Model { get; set; }
    }
}
