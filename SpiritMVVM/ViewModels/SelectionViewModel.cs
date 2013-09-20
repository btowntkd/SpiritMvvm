using System.Windows.Input;

namespace SpiritMVVM.ViewModels
{
    /// <summary>
    /// A ViewModel which wraps an object of a given type,
    /// providing an additional <see cref="IsSelected"/> state,
    /// for use in Selectable User Controls.
    /// </summary>
    /// <typeparam name="TItem">The type of the underlying item.</typeparam>
    public class SelectionViewModel<TItem> : ObservableObject
    {
        #region Private Members

        private bool _isSelected;
        private TItem _item;
        private RelayCommand _selectCommand;
        private RelayCommand _deselectCommand;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Default constructor creates a new instance of the <see cref="SelectionViewModel{T}"/>
        /// class in an un-selected state, with a default underlying instance.
        /// </summary>
        public SelectionViewModel()
            : this(default(TItem), false)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="SelectionViewModel{T}"/> class,
        /// with the given item value.
        /// </summary>
        /// <param name="item">The value to assign the item.</param>
        public SelectionViewModel(TItem item)
            : this(item, false)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="SelectionViewModel{T}"/> class,
        /// with the given item value.
        /// </summary>
        /// <param name="item">The value to assign the item.</param>
        /// <param name="isSelected">The default selected state to use.</param>
        public SelectionViewModel(TItem item, bool isSelected)
        {
            this.Item = item;
            this.IsSelected = isSelected;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Get or Set the selection state of the underlying item.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        /// <summary>
        /// Get or Set the value of the underlying item.
        /// </summary>
        public TItem Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        #endregion Public Properties

        #region Commands

        /// <summary>
        /// Command binding for selecting the item.
        /// </summary>
        [DependsOn("IsSelected")]
        public ICommand SelectCommand
        {
            get
            {
                if (_selectCommand == null)
                {
                    _selectCommand = new RelayCommand(
                        () => { IsSelected = true; },
                        () => !IsSelected);
                }
                return _selectCommand;
            }
        }

        /// <summary>
        /// Command binding for de-selecting the item.
        /// </summary>
        [DependsOn("IsSelected")]
        public ICommand DeselectCommand
        {
            get
            {
                if (_deselectCommand == null)
                {
                    _deselectCommand = new RelayCommand(
                        () => { IsSelected = false; },
                        () => IsSelected);
                }
                return _deselectCommand;
            }
        }

        #endregion Commands
    }
}