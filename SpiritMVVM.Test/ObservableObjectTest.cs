﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace SpiritMVVM.Test
{
    /// <summary>
    /// Unit tests for the <see cref="ObservableObject"/> class.
    /// </summary>
    [TestClass]
    public class ObservableObjectTest
    {
        internal class ObservableTestObject : ObservableObject
        {
            public void SetPropertyHelper(IPropertyNotifier helper)
            {
                PropertyNotifier = helper;
            }

            public IPropertyNotifier GetPropertyHelper()
            {
                return PropertyNotifier;
            }

            private int _testPropertyWithRef = 0;
            public int TestPropertyWithRef
            {
                get { return _testPropertyWithRef; }
                set { Set(ref _testPropertyWithRef, value); }
            }

            private object _testDependantProperty = null;
            [DependsOn("TestPropertyWithRef")]
            public object TestDependantProperty
            {
                get { return _testDependantProperty; }
                set { Set(ref _testDependantProperty, value); }
            }
        }

        /// <summary>
        /// Ensures that assigning a null value to the <see cref="ObservableObject.PropertyNotifier"/>
        /// property will result in an <see cref="ArgumentNullException"/>.
        /// </summary>
        [TestMethod]
        public void SetPropertyNotifier_NullValue_ThrowsArgumentNullException()
        {
            try
            {
                ObservableTestObject testObj = new ObservableTestObject();
                testObj.SetPropertyHelper(null);
                Assert.Fail("Expected exception");
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not the expected type");
            }
        }

        /// <summary>
        /// Ensures the Set method will result in an invocation of the currently-assigned
        /// <see cref="ObservableObject.PropertyNotifier"/> helper instance.
        /// </summary>
        [TestMethod]
        public void Set_CallsPropertyNotifier()
        {
            Mock<IPropertyNotifier> mockNotifier = new Mock<IPropertyNotifier>();

            ObservableTestObject testObj = new ObservableTestObject();
            testObj.SetPropertyHelper(mockNotifier.Object);
            testObj.TestPropertyWithRef = 12;

            var intRef = It.IsAny<int>();
            mockNotifier.Verify((x) => 
                x.SetProperty<int>(
                    ref intRef, 
                    It.IsAny<int>(), 
                    It.IsAny<Action<int, int>>(), 
                    It.IsAny<string>()), 
                Moq.Times.Once());
        }

        /// <summary>
        /// Ensures that the IReactOnDependencyChanged interface is correctly
        /// detected and executed when a dependency is modified.
        /// </summary>
        [TestMethod]
        public void RaisePropertyChangedDependants_WhenPropertyImplementsIReactOnDependencyChanged_CallsOnDependencyChanged()
        {
            bool callbackExecuted = false;
            var onDependencyChanged = new Mock<IReactOnDependencyChanged>();
            onDependencyChanged.Setup((x) => x.OnDependencyChanged()).Callback(() => callbackExecuted = true);
            var testObj = new ObservableTestObject();
            testObj.TestDependantProperty = onDependencyChanged.Object;
            testObj.TestPropertyWithRef = 12;


            Assert.IsTrue(callbackExecuted, "Expected OnDependencyChanged callback to be executed.");

        }
    }
}
