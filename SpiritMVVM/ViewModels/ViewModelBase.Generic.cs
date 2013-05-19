using SpiritMVVM.Messaging;

namespace SpiritMVVM.ViewModels
{
    /// <summary>
    /// A base implementation of the <see cref="IViewModel{T}"/> interface.
    /// </summary>
    /// <remarks>
    /// The <see cref="ViewModelBase{T}"/> class provides an <see cref="OnModelChanged"/>
    /// method to override, which executes any time the <see cref="Model"/> property changes.
    /// It also provides an <see cref="ViewModelBase.OnMessengerChanged"/> method to override,
    /// allowing users to unsubscribe from the old <see cref="IMessenger"/> instance,
    /// and subscribe to the new <see cref="IMessenger"/> instance.
    /// </remarks>
    /// <typeparam name="TModel">The type of the underlying ViewModel.</typeparam>
    public abstract class ViewModelBase<TModel> : ViewModelBase, IViewModel<TModel>
    {

        private TModel _model;

        /// <summary>
        /// Get or Set the value of the underlying model.
        /// </summary>
        public TModel Model
        {
            get { return _model; }
            set { Set(ref _model, value, OnModelChanged); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ViewModelBase()
            : base()
        {
            Model = default(TModel);
        }

        /// <summary>
        /// Constructor which specifies the <see cref="IMessenger"/> to assign the
        /// <see cref="ViewModelBase.Messenger"/> property.
        /// </summary>
        /// <param name="messenger">The messenger to use when broadcasting messages.</param>
        public ViewModelBase(IMessenger messenger)
            : base(messenger)
        {
            Model = default(TModel);
        }

        /// <summary>
        /// Constructor which specifies the initial value to 
        /// assign the <see cref="ViewModelBase{T}.Model"/> property.
        /// </summary>
        /// <param name="model">The model to assign.</param>
        public ViewModelBase(TModel model)
            : base()
        {
            Model = model;
        }

        /// <summary>
        /// Constructor which specifies the the initial value to 
        /// assign the <see cref="ViewModelBase{T}.Model"/> property,
        /// and the <see cref="IMessenger"/> instance to assign the
        /// <see cref="ViewModelBase.Messenger"/> property.
        /// </summary>
        /// <param name="model">The model to assign.</param>
        /// <param name="messenger">The messenger to use when broadcasting messages.</param>
        public ViewModelBase(TModel model, IMessenger messenger)
            : base(messenger)
        {
            Model = model;
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
