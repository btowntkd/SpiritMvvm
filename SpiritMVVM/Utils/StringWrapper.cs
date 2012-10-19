using System;
using System.Collections.Generic;

namespace SpiritMVVM.Utils
{
    /// <summary>
    /// A helper class which facilitates the conversion of an object to and from a String.
    ///
    /// Since the referenced item is accessed via Func delegates, the backing value can exist
    /// in a separate object, making this ideal for wrapping a Model's value from within a ViewModel.
    /// 
    /// When the <see cref="StringWrapper{T}.StringValue"/> property is changed, the class will
    /// attempt to parse its value and convert it into the underlying item's type.  
    /// If the attempt is unsuccessful, the IsValid property will be set to "False."
    /// If successful, the <see cref="StringWrapper{T}.IsValid"/> property will be "True."
    /// The referenced item's value will only be updated if the string is successfully parsed.
    /// 
    /// When the backing ref item is changed, the bound String value will automatically be updated
    /// to reflect the newest value.
    /// </summary>
    /// <typeparam name="T">The type of the referenced item.</typeparam>
    public class StringWrapper<T>
    {
        #region Private Fields

        private string _stringValue;
        private bool _isValid;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the <see cref="StringWrapper{T}"/> with the given get/set
        /// accessor methods, parse delegate, and ToString delegate.
        /// </summary>
        /// <param name="getter">The accessor used to get the referenced item's value.</param>
        /// <param name="setter">The accessor used to set the referenced item's value.</param>
        /// <param name="parseMethod">The method to use when parsing the string value into its
        /// underlying type.</param>
        /// <param name="toStringMethod">The method to use when converting the underlying type to
        /// a valid string value.</param>
        public StringWrapper(Func<T> getter, Action<T> setter, Func<string, T> parseMethod, Func<T, string> toStringMethod)
            : this(new Accessor<T>(getter, setter), parseMethod, toStringMethod)
        { }

        /// <summary>
        /// Creates an instance of the <see cref="StringWrapper{T}"/> with the given get/set                           
        /// accessor methods, parse delegate, and ToString delegate.
        /// </summary>
        /// <param name="itemAccessors">The accessors used to get or set the referenced item's value.</param>
        /// <param name="parseMethod">The method to use when parsing the string value into its
        /// underlying type.</param>
        /// <param name="toStringMethod">The method to use when converting the underlying type to
        /// a valid string value.</param>
        public StringWrapper(Accessor<T> itemAccessors, Func<string, T> parseMethod, Func<T, string> toStringMethod)
        {
            if (parseMethod == null)
                throw new ArgumentNullException("parseMethod");
            if (toStringMethod == null)
                throw new ArgumentNullException("toStringMethod");
            if (itemAccessors == null)
                throw new ArgumentNullException("itemAccessors");

            Accessor = itemAccessors;
            ParseStringMethod = parseMethod;
            ToStringMethod = toStringMethod;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get or Set the underlying Value.
        /// </summary>
        public virtual T Value
        {
            get { return Accessor.Getter(); }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(Accessor.Getter(), value))
                {
                    Accessor.Setter(value);
                    ResetString();
                }
            }
        }

        /// <summary>
        /// Get or Set the String that wraps the underlying value type.
        /// </summary>
        public virtual string StringValue
        {
            get
            {
                if (_stringValue == null)
                {
                    ResetString();
                }
                return _stringValue;
            }
            set
            {
                if (_stringValue != value)
                {
                    _stringValue = value;

                    try
                    {
                        Value = ParseStringMethod(_stringValue);
                        IsValid = true;
                    }
                    catch (Exception)
                    {
                        IsValid = false;
                    }
                }
            }
        }

        /// <summary>
        /// Get the Value indicating whether or not the String is valid
        /// (i.e. it can be successfully parsed into the underlying value type).
        /// </summary>
        public bool IsValid
        {
            get { return _isValid; }
            protected set 
            {
                _isValid = value; 
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the current string value to match the backing value.
        /// </summary>
        public void ResetString()
        {
            StringValue = ToStringMethod(Value);
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Get or Set the backing field's accessors.
        /// </summary>
        protected Accessor<T> Accessor { get; set; }

        /// <summary>
        /// Get or Set the ToString Method.
        /// </summary>
        protected Func<T, string> ToStringMethod { get; set; }

        /// <summary>
        /// Get or Set the Parse Method.
        /// </summary>
        protected Func<string, T> ParseStringMethod { get; set; }

        #endregion
    }
}
