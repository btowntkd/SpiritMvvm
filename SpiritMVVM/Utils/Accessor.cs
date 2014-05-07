using System;
using System.Reflection;

namespace SpiritMVVM.Utils
{
    /// <summary>
    /// Contains Getter and Setter methods for an externally-referenced value.
    /// </summary>
    public class Accessor<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        #region Constructors

        /// <summary>
        /// Create a new instance of the <see cref="Accessor{T}"/> object,
        /// with the given 'parent' host of the property, and the given <see cref="PropertyInfo"/> object.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="propInfo"></param>
        public Accessor(object parent, PropertyInfo propInfo)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (propInfo == null)
                throw new ArgumentNullException("propInfo");

            _getter = () => (T)propInfo.GetValue(parent);
            _setter = (x) => propInfo.SetValue(parent, x);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Accessor{T}"/> object, with the given Getter and Setter methods.
        /// </summary>
        /// <param name="getter">The method to use when retrieving the item's value.</param>
        /// <param name="setter">The method to use when setting the item's value.</param>
        public Accessor(Func<T> getter, Action<T> setter)
        {
            if (getter == null)
                throw new ArgumentNullException("getter");
            if (setter == null)
                throw new ArgumentNullException("setter");

            _getter = getter;
            _setter = setter;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Get or Set the underlying value using the getter or setter methods.
        /// </summary>
        public T Value
        {
            get { return _getter(); }
            set { _setter(value); }
        }

        #endregion Public Properties
    }
}