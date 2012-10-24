using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Utils;

namespace SpiritMVVM.Test.Utils
{
    /// <summary>
    /// Unit tests for the <see cref="StringWrapper{T}"/> class.
    /// </summary>
    [TestClass]
    public class StringWrapperTest
    {
        private const string ParseableIntegerString = "12";
        private const string UnparseableIntegerString = "HelloWorld";

        #region Constructor Tests

        /// <summary>
        /// Ensures that the constructor will throw an <see cref="ArgumentNullException"/>
        /// if provided with a null Getter method.
        /// </summary>
        [TestMethod]
        public void Constructor_NullGetter_ThrowsArgumentNullException()
        {
            try
            {
                int backingStore = 0;
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    null,
                    (x) => { backingStore = x; },
                    (x) => { return int.Parse(x); },
                    (x) => { return x.ToString(); });
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not expected type");
            }
        }

        /// <summary>
        /// Ensures that the constructor will throw an <see cref="ArgumentNullException"/>
        /// if provided with a null Setter method.
        /// </summary>
        [TestMethod]
        public void Constructor_NullSetter_ThrowsArgumentNullException()
        {
            try
            {
                int backingStore = 0;
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    () => { return backingStore; },
                    null,
                    (x) => { return int.Parse(x); },
                    (x) => { return x.ToString(); });
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not expected type");
            }
        }

        /// <summary>
        /// Ensures that the constructor will throw an <see cref="ArgumentNullException"/>
        /// if provided with a null Parse method.
        /// </summary>
        [TestMethod]
        public void Constructor_NullParseMethod_ThrowsArgumentNullException()
        {
            try
            {
                int backingStore = 0;
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    () => { return backingStore; },
                    (x) => { backingStore = x; },
                    null,
                    (x) => { return x.ToString(); });
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not expected type");
            }
        }

        /// <summary>
        /// Ensures that the constructor will throw an <see cref="ArgumentNullException"/>
        /// if provided with a null ToString method.
        /// </summary>
        [TestMethod]
        public void Constructor_NullToStringMethod_ThrowsArgumentNullException()
        {
            try
            {
                int backingStore = 0;
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    () => { return backingStore; },
                    (x) => { backingStore = x; },
                    (x) => { return int.Parse(x); },
                    null);
                Assert.Fail("Expected exception to be thrown");
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not expected type");
            }
        }

        /// <summary>
        /// Ensures the constructor is successful when provided with valid parameters.
        /// </summary>
        [TestMethod]
        public void Constructor_ValidArgs_Success()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });
        }

        #endregion

        #region Get/Set Value Tests

        /// <summary>
        /// Ensures the Value property returns the value directly from the "Get" delegate.
        /// </summary>
        [TestMethod]
        public void GetValue_ReturnsValueFromGetDelegate()
        {
            int backingStore = 100;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            int returnedValue = wrapper.Value;

            Assert.AreEqual(backingStore, returnedValue, "Wrapper did not return the value from the Get accessor");
        }

        /// <summary>
        /// Ensures the Value property assigns the value directly to the "Set" delegate.
        /// </summary>
        [TestMethod]
        public void SetValue_PassesValueToSetDelegate()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            int newValue = 12;
            wrapper.Value = newValue;

            Assert.AreEqual(backingStore, newValue, "Wrapper did not pass value to the Set accessor");
        }

        #endregion

        #region Get/Set StringValue Tests

        /// <summary>
        /// Ensures the StringValue property returns the initial value of the ToString delegate
        /// in its initial state.
        /// </summary>
        [TestMethod]
        public void GetStringValue_DefaultStringValueIsInitialValueString()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            string initialValue = wrapper.StringValue;

            Assert.AreEqual(initialValue, "0", "Expected default StringValue to be equal to the Value string.");
        }

        /// <summary>
        /// Ensures the StringValue property returns the value which is earlier assigned to it.
        /// </summary>
        [TestMethod]
        public void GetSetStringValue_WithInvalidString_AssignsAndReturnsStringValue()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            string expectedValue = UnparseableIntegerString;
            wrapper.StringValue = expectedValue;
            string actualValue = wrapper.StringValue;

            Assert.AreEqual(expectedValue, actualValue, "Expected and Actual values should have been equal");
            
        }

        /// <summary>
        /// Ensures the StringValue property returns the value which is earlier assigned to it.
        /// </summary>
        [TestMethod]
        public void GetSetStringValue_WithValidString_AssignsAndReturnsStringValue()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            string expectedValue = ParseableIntegerString;
            wrapper.StringValue = expectedValue;
            string actualValue = wrapper.StringValue;

            Assert.AreEqual(expectedValue, actualValue, "Expected and Actual values should have been equal");
        }

        /// <summary>
        /// Ensures the StringValue property does not effect the Value property 
        /// when assigned an invalid/unparseable string.
        /// </summary>
        [TestMethod]
        public void SetStringValue_WithInvalidString_DoesNotChangeValue()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            int beforeValue = wrapper.Value;
            string expectedValue = UnparseableIntegerString;
            wrapper.StringValue = expectedValue;
            int afterValue = wrapper.Value;

            Assert.AreEqual(beforeValue, afterValue, "Before/After values should not have changed.");
        }

        /// <summary>
        /// Ensures the StringValue property directly assigns the Value property 
        /// when assigned a valid/parseable string.
        /// </summary>
        [TestMethod]
        public void SetStringValue_WithValidString_ChangesValue()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            int beforeValue = wrapper.Value;
            string expectedValue = ParseableIntegerString;
            wrapper.StringValue = expectedValue;
            int afterValue = wrapper.Value;

            Assert.AreNotEqual(beforeValue, afterValue, "Before/After values should have changed.");
        }

        #endregion

        #region ResetString Tests

        /// <summary>
        /// Ensures that the ResetString method calls the ToString delegate provided in the constructor.
        /// </summary>
        [TestMethod]
        public void ResetString_CallsToStringDelegate()
        {
            int backingStore = 0;
            int toStringDelegateExecutedCount = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { toStringDelegateExecutedCount++; return x.ToString(); });

            //We take an initial count, because the current implementation of
            //string wrapper calls "ResetString" in the constructor, but
            //this test needs to be implementation-agnostic.
            int initialCount = toStringDelegateExecutedCount;

            wrapper.ResetString();
            Assert.IsTrue((toStringDelegateExecutedCount > initialCount), "Expected ToString delegate to be executed when ResetString is called.");
        }

        #endregion

        #region IsValid Tests

        /// <summary>
        /// Ensures IsValid is set to true in its initial state.
        /// </summary>
        [TestMethod]
        public void IsValid_InitialStateIsTrue()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            Assert.IsTrue(wrapper.IsValid, "Expected IsValid to be true in initial state.");
        }

        /// <summary>
        /// Ensures IsValid is set to false, after assinging an invalid/unparseable string to the StringValue property.
        /// </summary>
        [TestMethod]
        public void IsValid_AfterAssigningInvalidString_IsFalse()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            wrapper.StringValue = UnparseableIntegerString;
            Assert.IsFalse(wrapper.IsValid, "Expected IsValid to be false when assigning an invalid string.");
        }

        /// <summary>
        /// Ensures IsValid is reset to true, after assinging a valid/parseable string to the StringValue property.
        /// </summary>
        [TestMethod]
        public void IsValid_AfterAssigningValidString_IsTrue()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            wrapper.StringValue = UnparseableIntegerString; //ensure we start with IsValid == false
            wrapper.StringValue = ParseableIntegerString;

            Assert.IsTrue(wrapper.IsValid, "Expected IsValid to be true after assigning a valid string.");
        }

        /// <summary>
        /// Ensures IsValid is reset to true, after the ResetString method is executed.
        /// </summary>
        [TestMethod]
        public void IsValid_AfterCallingResetString_IsTrue()
        {
            int backingStore = 0;
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return int.Parse(x); },
                (x) => { return x.ToString(); });

            wrapper.StringValue = UnparseableIntegerString; //ensure that IsValid is false, momentarily
            wrapper.ResetString();
            Assert.IsTrue(wrapper.IsValid, "Expected IsValid to be true.");
        }

        #endregion
    }
}
