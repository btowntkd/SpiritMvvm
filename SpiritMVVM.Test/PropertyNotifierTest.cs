using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiritMVVM.Test
{
    [TestClass]
    public class PropertyNotifierTest
    {
        #region Constructor Tests

        [TestMethod]
        public void Constructor_NullPropertyChangedAction_ThrowsException()
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

        [TestMethod]
        public void Constructor_AllValidArgs_Success()
        {
            Action<string> doNothingAction = new Action<string>((s) => { });
            PropertyNotifier notifier = new PropertyNotifier(doNothingAction);
        }

        #endregion

        #region Set (with ref Argument) Tests

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
            Assert.AreEqual(propertyChangedActionValue, newValue, "Expected PropertyChanged action to be executed after the value was changed.");
        }

        ////Unit test removed because INotifyPropertyChanging doesn't exist in PCL yet.
        //[TestMethod]
        //public void Set_WithRefArgument_Changed_ExecutesPropertyChangingAction_BeforeChange()
        //{
        //    bool notifierExecuted = false;
        //    int oldValue = 0;
        //    int backingStore = oldValue;
        //    int newValue = 12;
        //    int propertyChangingActionValue = 0;
        //    PropertyNotifier notifier = new PropertyNotifier((propName) => { /* Do nothing */ },
        //        (propName) =>
        //        {
        //            propertyChangingActionValue = backingStore;
        //            notifierExecuted = true;
        //        });

        //    notifier.SetProperty(ref backingStore, newValue);

        //    Assert.IsTrue(notifierExecuted, "Expected notifier delegate to be executed.");
        //    Assert.AreEqual(propertyChangingActionValue, oldValue, "Expected PropertyChanging action to be executed before the value was changed.");
        //}

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

        ////Unit test removed because INotifyPropertyChanging doesn't exist in PCL yet.
        //[TestMethod]
        //public void Set_WithRefArgument_NotChanged_DoesNotExecutePropertyChangingAction()
        //{
        //    bool notifierExecuted = false;
        //    PropertyNotifier notifier = new PropertyNotifier((propName) => { },
        //        (propName) =>
        //        {
        //            notifierExecuted = true;
        //        });

        //    int backingStore = 0;
        //    int newValue = 0;
        //    notifier.SetProperty(ref backingStore, newValue);

        //    Assert.IsFalse(notifierExecuted, "Did not expect PropertyChanging action to be executed.");
        //}

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

        [TestMethod]
        public void Set_WithAccessor_Changed_ExecutesNotifier()
        {
            bool notifierExecuted = false;
            PropertyNotifier notifier = new PropertyNotifier((propName) =>
            {
                notifierExecuted = true;
            });

            int backingStore = 0;
            Accessor<int> accessor = new Accessor<int>(() => backingStore, (x) => backingStore = x);
            int newValue = 12;
            notifier.SetProperty(accessor, newValue);

            Assert.IsTrue(notifierExecuted, "Expected notifier delegate to be executed.");
        }

        [TestMethod]
        public void Set_WithAccessor_NotChanged_DoesNotExecuteNotifier()
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

        [TestMethod]
        public void Set_WithAccessor_Changed_ProvidesCorrectPropertyNameToNotifier()
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
