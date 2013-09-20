﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;

namespace SpiritMVVM.Test
{
    /// <summary>
    /// Unit tests for the <see cref="PropertyListener"/> class.
    /// </summary>
    [TestClass]
    public class PropertyListenerTest
    {
        internal class SimpleViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = null;

            private int _testProperty = 0;

            public int TestProperty
            {
                get { return _testProperty; }
                set { _testProperty = value; RaisePropertyChanged("TestProperty"); }
            }

            public void RaisePropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Ensures the constructor will throw an <see cref="ArgumentNullException"/> when
        /// provided with a null parent parameter.
        /// </summary>
        [TestMethod]
        public void Constructor_NullParent_ThrowsArgumentNullException()
        {
            try
            {
                PropertyListener listener = new PropertyListener(null);
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not expected type.");
            }
        }

        /// <summary>
        /// Ensures the constructor is successful when provided with valid parameters.
        /// </summary>
        [TestMethod]
        public void Constructor_ValidArgs_Success()
        {
            var viewModel = new SimpleViewModel();
            PropertyListener listener = new PropertyListener(viewModel);
        }

        /// <summary>
        /// Ensures that a listener action is executed when the monitored parent object
        /// raises a matching <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        [TestMethod]
        public void AddListener_WhenPropertyChanged_ExecutesAction()
        {
            var viewModel = new SimpleViewModel();
            PropertyListener listener = new PropertyListener(viewModel);

            bool actionExecuted = false;
            listener.AddListener<int>("TestProperty", (x) =>
            {
                actionExecuted = true;
            });
            viewModel.TestProperty = 12;

            Assert.IsTrue(actionExecuted, "Expected notification action to be executed");
        }

        /// <summary>
        /// Ensures that a listener action is provided with the correct, current value
        /// of a monitored property.
        /// </summary>
        [TestMethod]
        public void AddListener_WhenPropertyChanged_ProvidesValueToAction()
        {
            var viewModel = new SimpleViewModel();
            PropertyListener listener = new PropertyListener(viewModel);

            int expectedValue = 12;
            int actualValue = 0;
            listener.AddListener<int>("TestProperty", (x) =>
            {
                actualValue = x;
            });
            viewModel.TestProperty = expectedValue;

            Assert.AreEqual(expectedValue, actualValue, "Value delivered to listener was not correct.");
        }

        /// <summary>
        /// Ensures that a listener action is not executed when non-matching properties
        /// are changed.
        /// </summary>
        [TestMethod]
        public void RemoveListeners_WhenPropertyChanged_DoesNotExecuteAction()
        {
            var viewModel = new SimpleViewModel();
            PropertyListener listener = new PropertyListener(viewModel);

            bool actionExecuted = false;
            listener.AddListener<int>("TestProperty", (x) =>
            {
                actionExecuted = true;
            });
            listener.RemoveListeners("TestProperty");
            viewModel.TestProperty = 12;

            Assert.IsFalse(actionExecuted, "Notification action should not have been executed.");
        }
    }
}