using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Utils;
using System;

namespace SpiritMVVM.Test.Utils
{
    [TestClass]
    public class AccessorTest
    {
        /// <summary>
        /// Ensures that a constructor called with a null Getter will throw an <see cref="ArgumentNullException"/>.
        /// </summary>
        [TestMethod]
        public void Constructor_GetterIsNull_ThrowsException()
        {
            try
            {
                int backingStore;
                Accessor<int> accessor = new Accessor<int>(null, (x) => backingStore = x);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (ArgumentNullException)
            {
                //PASS
            }
            catch (Exception)
            {
                Assert.Fail("Exception does not match expected type.");
            }
        }

        /// <summary>
        /// Ensures that a constructor called with a null Setter will throw an <see cref="ArgumentNullException"/>.
        /// </summary>
        [TestMethod]
        public void Constructor_SetterIsNull_ThrowsException()
        {
            try
            {
                int backingStore = 0;
                Accessor<int> accessor = new Accessor<int>(() => backingStore, null);
                Assert.Fail("Expected exception to be thrown.");
            }
            catch (ArgumentNullException)
            {
                //PASS
            }
            catch (Exception)
            {
                Assert.Fail("Exception does not match expected type.");
            }
        }

        /// <summary>
        /// Ensures that a constructor called with valid arguments will not throw an exception.
        /// </summary>
        [TestMethod]
        public void Constructor_ValidArgs_Success()
        {
            int backingStore = 0;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
        }

        /// <summary>
        /// Ensures that getting the Value property calls the given getter method.
        /// </summary>
        [TestMethod]
        public void GetValue_CallsGetter()
        {
            bool getterCalled = false;
            Accessor<int> accessor = new Accessor<int>(
            () => { getterCalled = true; return 0; },
            (x) => { /* Do nothing */ });

            //Call the Value's "Get" method
            int getVal = accessor.Value;

            Assert.IsTrue(getterCalled, "Expected Getter method to be called");
        }

        /// <summary>
        /// Ensures that setting the Value property calls the given setter method.
        /// </summary>
        [TestMethod]
        public void SetValue_CallsSetter()
        {
            bool setterCalled = false;
            Accessor<int> accessor = new Accessor<int>(
                () => { return 0; },
                (x) => { setterCalled = true; });

            //Call the Value's "Set" method
            accessor.Value = 0;

            Assert.IsTrue(setterCalled, "Expected Setter method to be called");
        }
    }
}
