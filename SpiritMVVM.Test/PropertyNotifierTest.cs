using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Utils;
using System.Threading.Tasks;

namespace SpiritMVVM.Test
{
    /// <summary>
    /// Unit tests for the <see cref="PropertyNotifier"/> class.
    /// </summary>
    [TestClass]
    public class PropertyNotifierTest
    {
        #region Constructor Tests

        /// <summary>
        /// Ensures that the constructor will throw an <see cref="ArgumentNullException"/>
        /// when the provided propertyChangedAction is null.
        /// </summary>
        [TestMethod]
        public void Constructor_NullPropertyChangedAction_ThrowsArgumentNullException()
        {
            try
            {
                PropertyNotifier notifier = new PropertyNotifier(null);
            }
            catch (ArgumentNullException)
            {
                //PASS
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not the expected type.");
            }
        }

        ////Unit test removed because INotifyPropertyChanging doesn't exist in PCL yet.
        //[TestMethod]
        //public void Constructor_NullPropertyChangingAction_Success()
        //{
        //    PropertyNotifier notifier = new PropertyNotifier((s) => { /* Do nothing */ });
        //}

        /// <summary>
        /// Ensures that the constructor is successful when provided with valid parameters.
        /// </summary>
        [TestMethod]
        public void Constructor_AllValidArgs_Success()
        {
            Action<string> doNothingAction = new Action<string>((s) => { });
            PropertyNotifier notifier = new PropertyNotifier(doNothingAction);
        }

        #endregion

        #region Set (with ref Argument) Tests

        /// <summary>
        /// Ensures that the Set method will execute the instance-wide propertyChangedAction
        /// after the property's value has changed.
        /// </summary>
        [TestMethod]
        public void Set_WithRefArgument_Changed_ExecutesPropertyChangedAction_AfterChange()
        {
            bool notifierExecuted = false;
            int backingStore = 0;
            int newValue = 12;
            int propertyChangedActionValue = 0;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                propertyChangedActionValue = backingStore;
                notifierExecuted = true;
            });

            notifier.SetProperty(ref backingStore, newValue);
            
            Assert.IsTrue(notifierExecuted, "Expected notifier delegate to be executed.");
            Assert.AreEqual(propertyChangedActionValue, newValue, 
                "Expected PropertyChanged action to be executed after the value was changed,"
                + " but it was executed before the value was changed.");
        }

        /// <summary>
        /// Ensures that the instance-wide PropertyChanged Action is not called, if the property is not changed.
        /// </summary>
        [TestMethod]
        public void Set_WithRefArgument_NotChanged_DoesNotExecutePropertyChangedAction()
        {
            bool notifierExecuted = false;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                notifierExecuted = true;
            });

            int backingStore = 0;
            int newValue = 0;
            notifier.SetProperty(ref backingStore, newValue);

            Assert.IsFalse(notifierExecuted, "Did not expect PropertyChanged action to be executed.");
        }

        /// <summary>
        /// Ensures that when the instance-wide propertyChanged Action is executed, it is provided with the
        /// correct property name.
        /// </summary>
        [TestMethod]
        public void Set_WithRefArgument_Changed_ProvidesCorrectPropertyNameToPropertyChangedAction()
        {
            //Use a guid as a property name, to ensure randomness
            string expectedPropertyName = Guid.NewGuid().ToString();
            string receivedPropertyName = string.Empty;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                receivedPropertyName = propName;
            });

            int backingStore = 0;
            int newValue = 12;
            notifier.SetProperty(ref backingStore, newValue, null, expectedPropertyName);

            Assert.AreEqual(expectedPropertyName, receivedPropertyName, "Property names do not match.");
        }

        /// <summary>
        /// Ensures that the <see cref="PropertyNotifier"/> can execute the callback,
        /// even if the "Set" method is called from another thread.
        /// </summary>
        [TestMethod]
        public void Set_WithRefArgument_ChangedOnDifferentThread_ExecutesCallback()
        {
            bool notifierExecuted = false;
            int backingStore = 0;
            int newValue = 12;
            int propertyChangedActionValue = 0;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                propertyChangedActionValue = backingStore;
                notifierExecuted = true;
            });

            Task.Factory.StartNew(() => notifier.SetProperty(ref backingStore, newValue)).Wait();
            
            Assert.IsTrue(notifierExecuted, "Expected notifier delegate to be executed.");
            Assert.AreEqual(propertyChangedActionValue, newValue, 
                "Expected PropertyChanged action to be executed after the value was changed,"
                + " but it was executed before the value was changed.");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is executed, when the property changes.
        /// </summary>
        [TestMethod]
        public void Set_WithRefArgumentAndCallback_Changed_ExecutesCallback()
        {
            //Use a guid as a property name, to ensure randomness
            PropertyNotifier notifier = new PropertyNotifier((propName) => { /* Do nothing */ });

            bool callbackExecuted = false;
            int backingStore = 0;
            int newValue = 12;
            notifier.SetProperty(ref backingStore, newValue, (oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            Assert.IsTrue(callbackExecuted, "Callback was not executed.");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is not executed,
        /// when the property does not change.
        /// </summary>
        [TestMethod]
        public void Set_WithRefArgumentAndCallback_NotChanged_DoesNotExecuteCallback()
        {
            //Use a guid as a property name, to ensure randomness
            PropertyNotifier notifier = new PropertyNotifier((propName) => { /* Do nothing */ });

            bool callbackExecuted = false;
            int backingStore = 12;
            int newValue = 12;
            notifier.SetProperty(ref backingStore, newValue, (oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            Assert.IsFalse(callbackExecuted, "Callback was executed, but shouldn't.");
        }

        /// <summary>
        /// Ensures that when the propertyChangedCallback parameter is executed, it is provided with the
        /// correct old/new property values.
        /// </summary>
        [TestMethod]
        public void Set_WithRefArgumentAndCallback_Changed_ProvidesCorrectOldAndNewValues()
        {
            //Use a guid as a property name, to ensure randomness
            PropertyNotifier notifier = new PropertyNotifier((propName) => { /* Do nothing */ });

            int expectedOldValue = 0;
            int expectedNewValue = 12;
            int receivedOldValue = -1;
            int receivedNewValue = -1;
            int backingStore = 0;
            notifier.SetProperty(ref backingStore, expectedNewValue, (oldVal, newVal) =>
            {
                receivedOldValue = oldVal;
                receivedNewValue = newVal;
            });

            Assert.AreEqual(expectedOldValue, receivedOldValue, "'Old' values do not match.");
            Assert.AreEqual(expectedNewValue, receivedNewValue, "'New' values do not match.");
        }

        #endregion

        #region Set (with Accessor<T>) Tests

        /// <summary>
        /// Ensures that the Set method will execute the instance-wide propertyChangedAction
        /// after the property's value has changed.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_Changed_ExecutesPropertyChangedAction_AfterChange()
        {
            int backingStore = 0;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
            int newValue = 12;
            bool notifierExecuted = false;
            int propertyChangedActionValue = 0;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                propertyChangedActionValue = backingStore;
                notifierExecuted = true;
            });

            notifier.SetProperty(accessor, newValue);

            Assert.IsTrue(notifierExecuted, "Expected notifier delegate to be executed.");
            Assert.AreEqual(propertyChangedActionValue, newValue,
                "Expected PropertyChanged action to be executed after the value was changed,"
                + " but it was executed before the value was changed.");
        }

        /// <summary>
        /// Ensures that the instance-wide PropertyChanged Action is not called, if the property is not changed.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_NotChanged_DoesNotExecutePropertyChangedAction()
        {
            bool notifierExecuted = false;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                notifierExecuted = true;
            });

            int backingStore = 0;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
            int newValue = 0;
            notifier.SetProperty(accessor, newValue);

            Assert.IsFalse(notifierExecuted, "Did not expect notifier delegate to be executed.");
        }

        /// <summary>
        /// Ensures that when the instance-wide propertyChanged Action is executed, it is provided with the
        /// correct property name.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_Changed_ProvidesCorrectPropertyNameToPropertyChangedAction()
        {
            //Use a guid as a property name, to ensure randomness
            string expectedPropertyName = Guid.NewGuid().ToString();
            string receivedPropertyName = string.Empty;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                receivedPropertyName = propName;
            });

            int backingStore = 0;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
            int newValue = 12;
            notifier.SetProperty(accessor, newValue, null, expectedPropertyName);

            Assert.AreEqual(expectedPropertyName, receivedPropertyName, "Property names do not match.");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is executed, when the property changes.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_Changed_ExecutesCallback()
        {
            //Use a guid as a property name, to ensure randomness
            PropertyNotifier notifier = new PropertyNotifier((propName) => { /* Do nothing */ });

            bool callbackExecuted = false;
            int backingStore = 0;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
            int newValue = 12;
            notifier.SetProperty(accessor, newValue, (oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            Assert.IsTrue(callbackExecuted, "Callback was not executed.");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is not executed,
        /// when the property does not change.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_NotChanged_DoesNotExecuteCallback()
        {
            //Use a guid as a property name, to ensure randomness
            PropertyNotifier notifier = new PropertyNotifier((propName) => { /* Do nothing */ });

            bool callbackExecuted = false;
            int backingStore = 12;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
            int newValue = 12;
            notifier.SetProperty(accessor, newValue, (oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            Assert.IsFalse(callbackExecuted, "Callback was executed, but shouldn't.");
        }

        /// <summary>
        /// Ensures that when the propertyChangedCallback parameter is executed, it is provided with the
        /// correct old/new property values.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_Changed_ProvidesCorrectOldAndNewValues()
        {
            //Use a guid as a property name, to ensure randomness
            PropertyNotifier notifier = new PropertyNotifier((propName) => { /* Do nothing */ });

            int expectedOldValue = 0;
            int expectedNewValue = 12;
            int receivedOldValue = -1;
            int receivedNewValue = -1;
            int backingStore = 0;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
            notifier.SetProperty(accessor, expectedNewValue, (oldVal, newVal) =>
            {
                receivedOldValue = oldVal;
                receivedNewValue = newVal;
            });

            Assert.AreEqual(expectedOldValue, receivedOldValue, "'Old' values do not match.");
            Assert.AreEqual(expectedNewValue, receivedNewValue, "'New' values do not match.");
        }

        #endregion
    }
}
