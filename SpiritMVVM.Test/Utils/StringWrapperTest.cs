using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Utils;

namespace SpiritMVVM.Test.Utils
{
    [TestClass]
    public class StringWrapperTest
    {
        #region Constructor Tests

        [TestMethod]
        public void Constructor_NullGetter_ThrowsArgumentNullException()
        {
            try
            {
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    null,
                    (x) => { },
                    (x) => { return 0; },
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

        [TestMethod]
        public void Constructor_NullSetter_ThrowsArgumentNullException()
        {
            try
            {
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    () => { return 0; },
                    null,
                    (x) => { return 0; },
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

        [TestMethod]
        public void Constructor_NullParseMethod_ThrowsArgumentNullException()
        {
            try
            {
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    () => { return 0; },
                    (x) => { },
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

        [TestMethod]
        public void Constructor_NullToStringMethod_ThrowsArgumentNullException()
        {
            try
            {
                StringWrapper<int> wrapper = new StringWrapper<int>(
                    () => { return 0; },
                    (x) => { },
                    (x) => { return 0; },
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

        [TestMethod]
        public void Constructor_ValidArgs_Success()
        {
            StringWrapper<int> wrapper = new StringWrapper<int>(
                () => { return 0; },
                (x) => { },
                (x) => { return 0; },
                (x) => { return x.ToString(); });
        }

        #endregion

        #region Get/Set tests

        [TestMethod]
        public void ValueGet_ReturnsValueFromGetDelegate()
        {
            object backingStore = new object();

            StringWrapper<object> wrapper = new StringWrapper<object>(
                () => { return backingStore; },
                (x) => { },
                (x) => { return null; },
                (x) => { return x.ToString(); });

            object returnedValue = wrapper.Value;

            Assert.AreEqual(backingStore, returnedValue, "Wrapper did not return the value from the Get accessor");
        }

        [TestMethod]
        public void ValueSet_PassesValueToSetDelegate()
        {
            object backingStore = new object();

            StringWrapper<object> wrapper = new StringWrapper<object>(
                () => { return backingStore; },
                (x) => { backingStore = x; },
                (x) => { return null; },
                (x) => { return x.ToString(); });

            object newValue = new object();
            wrapper.Value = newValue;

            Assert.AreEqual(backingStore, newValue, "Wrapper did not pass value to the Set accessor");
        }

        #endregion
    }
}
