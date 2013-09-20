using System;

namespace SpiritMVVM.Utils
{
    /// <summary>
    /// Contains Getter and Setter methods for an externally-referenced value.
    /// </summary>
    public class Accessor<T>
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Accessor{T}"/> object, with the given Getter and Setter methods.
        /// </summary>
        /// <param name="getter">The method to use when retrieving the item's value.</param>
        /// <param name="setter">The method to use when setting the item's value.</param>
        public Accessor(Func<T> getter, Action<T> setter)
        {
            if (getter == null)
            {
                throw new ArgumentNullException("getter");
            }
            if (setter == null)
            {
                throw new ArgumentNullException("setter");
            }

            Getter = getter;
            Setter = setter;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Get or Set the underlying value using the getter or setter methods.
        /// </summary>
        public T Value
        {
            get { return Getter(); }
            set { Setter(value); }
        }

        /// <summary>
        /// Get the Getter method.
        /// </summary>
        public Action<T> Setter { get; private set; }

        /// <summary>
        /// Get the Setter method
        /// </summary>
        public Func<T> Getter { get; private set; }

        #endregion Public Properties
    }
}