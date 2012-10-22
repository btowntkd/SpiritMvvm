
namespace SpiritMVVM.ViewModels
{
    /// <summary>
    /// A base implementation of the <see cref="IViewModel{T}"/> interface,
    /// providing <see cref="OnModelChanged"/> method to override,
    /// which executes any time the <see cref="Model"/> property changes.
    /// </summary>
    /// <typeparam name="TModel">The type of the underlying ViewModel.</typeparam>
    public class ViewModelBase<TModel> : ViewModelBase, IViewModel<TModel>
    {
        private TModel _model;

        /// <summary>
        /// Get or Set the value of the underlying model.
        /// </summary>
        public TModel Model
        {
            get { return _model; }
            set { Set(ref _model, value, (x, y) => OnModelChanged(x, y)); }
        }

        /// <summary>
        /// Executes whenever the Model property changes.
        /// </summary>
        /// <param name="oldModel">The value of the previous model.</param>
        /// <param name="newModel">The new, current model.</param>
        protected virtual void OnModelChanged(TModel oldModel, TModel newModel)
        {

        }
    }
}
