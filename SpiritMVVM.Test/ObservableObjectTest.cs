using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SpiritMVVM.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            public ObservableTestObject()
            {
                //Set up Accessor
                _backingStoreAccessor = new Accessor<object>(
                    () => _backingStore,
                    (x) => _backingStore = x);
            }

            private object _backingStore = null;
            private Accessor<object> _backingStoreAccessor = null;

            public object TestRefProperty
            {
                get { return _backingStore; }
                set { Set(ref _backingStore, value); }
            }

            public object TestAccessorProperty
            {
                get { return _backingStoreAccessor; }
                set { Set(_backingStoreAccessor, value); }
            }

            public object TestRefPropertyWithCallback
            {
                get { return _backingStore; }
                set { Set(ref _backingStore, value, _callback); }
            }

            public object TestAccessorPropertyWithCallback
            {
                get { return _backingStoreAccessor; }
                set { Set(_backingStoreAccessor, value, _callback); }
            }

            PropertyChangedCallback<object> _callback = null;
            public void SetCallback(PropertyChangedCallback<object> callback)
            {
                _callback = callback;
            }

            private object _dependencyProperty = null;
            public object DependencyProperty
            {
                get { return _dependencyProperty; }
                set { Set(ref _dependencyProperty, value); }
            }

            private object _reactionProperty;
            [DependsOn("DependencyProperty")]
            public object ReactionProperty
            {
                get { return _reactionProperty; }
                set { Set(ref _reactionProperty, value); }
            }

            [DependsOn("DependencyProperty")]
            public object DirectlyDependantProperty
            {
                get { return _dependencyProperty; }
            }

            [DependsOn("DirectlyDependantProperty")]
            public object IndirectlyDependantProperty
            {
                get { return _dependencyProperty; }
            }

            public object DirectlyDependantFluentProperty
            {
                get { return _dependencyProperty; }
            }

            public object IndirectlyDependantFluentProperty
            {
                get { return _dependencyProperty; }
            }
        }

        #region Set (with ref Argument) Tests

        /// <summary>
        /// Ensures that the Set method will execute the instance-wide propertyChangedAction
        /// after the property's value has changed.
        /// </summary>
        [TestMethod]
        public void Set_WithRef_Changed_RaisesPropertyChanged()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            bool eventRaised = false;
            testObj.PropertyChanged += (obj, args) =>
            {
                eventRaised = true;
            };

            testObj.TestRefProperty = new object();

            Assert.IsTrue(eventRaised, "Expected event was not raised");
        }

        /// <summary>
        /// Ensures that the instance-wide PropertyChanged Action is not called, if the property is not changed.
        /// </summary>
        [TestMethod]
        public void Set_WithRef_NotChanged_DoesNotRaisePropertyChanged()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            object objValue = new object();
            bool eventRaised = false;
            testObj.TestRefProperty = objValue;
            testObj.PropertyChanged += (obj, args) =>
            {
                eventRaised = true;
            };

            testObj.TestRefProperty = objValue;

            Assert.IsFalse(eventRaised, "Did not expect PropertyChanged action to be executed.");
        }

        /// <summary>
        /// Ensures that the Set method can raise the PropertyChanged event,
        /// even if the "Set" method is called from another thread.
        /// </summary>
        [TestMethod]
        public void Set_WithRef_ChangedOnDifferentThread_RaisesPropertyChanged()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            bool eventRaised = false;
            testObj.PropertyChanged += (obj, args) =>
            {
                eventRaised = true;
            };
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(50);
                testObj.TestRefProperty = 12;
            }));

            thread.Start();
            thread.Join();

            Assert.IsTrue(eventRaised, "Expected event was not raised");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is executed, when the property changes.
        /// </summary>
        [TestMethod]
        public void Set_WithRefAndCallback_Changed_ExecutesCallback()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            bool callbackExecuted = false;
            testObj.SetCallback((oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            testObj.TestRefPropertyWithCallback = new object();

            Assert.IsTrue(callbackExecuted, "Callback was not executed");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is not executed,
        /// when the property does not change.
        /// </summary>
        [TestMethod]
        public void Set_WithRefAndCallback_NotChanged_DoesNotExecuteCallback()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            var objValue = new object();
            testObj.TestRefPropertyWithCallback = objValue;
            bool callbackExecuted = false;
            testObj.SetCallback((oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            testObj.TestRefPropertyWithCallback = objValue;

            Assert.IsFalse(callbackExecuted, "Callback was executed, but shouldn't have.");
        }

        /// <summary>
        /// Ensures that when the propertyChangedCallback parameter is executed, it is provided with the
        /// correct old/new property values.
        /// </summary>
        [TestMethod]
        public void Set_WithRefAndCallback_Changed_ProvidesCorrectOldAndNewValues()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            object expectedOld = new object();
            object expectedNew = new object();
            object actualOld = null;
            object actualNew = null;
            testObj.TestRefPropertyWithCallback = expectedOld;
            testObj.SetCallback((oldVal, newVal) =>
            {
                actualOld = oldVal;
                actualNew = newVal;
            });

            testObj.TestRefPropertyWithCallback = expectedNew;

            Assert.AreEqual(expectedOld, actualOld, "'Old' values do not match.");
            Assert.AreEqual(expectedNew, actualNew, "'New' values do not match.");
        }

        #endregion Set (with ref Argument) Tests

        #region Set (with Accessor<T>) Tests

        /// <summary>
        /// Ensures that the Set method will execute the instance-wide propertyChangedAction
        /// after the property's value has changed.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_Changed_RaisesPropertyChanged()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            bool eventRaised = false;
            testObj.PropertyChanged += (obj, args) =>
            {
                eventRaised = true;
            };

            testObj.TestAccessorProperty = new object();

            Assert.IsTrue(eventRaised, "Expected event was not raised");
        }

        /// <summary>
        /// Ensures that the instance-wide PropertyChanged Action is not called, if the property is not changed.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_NotChanged_DoesNotRaisePropertyChanged()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            object objValue = new object();
            bool eventRaised = false;
            testObj.TestAccessorProperty = objValue;
            testObj.PropertyChanged += (obj, args) =>
            {
                eventRaised = true;
            };

            testObj.TestAccessorProperty = objValue;

            Assert.IsFalse(eventRaised, "Did not expect PropertyChanged action to be executed.");
        }

        /// <summary>
        /// Ensures that the Set method can raise the PropertyChanged event,
        /// even if the "Set" method is called from another thread.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessor_ChangedOnDifferentThread_RaisesPropertyChanged()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            bool eventRaised = false;
            testObj.PropertyChanged += (obj, args) =>
            {
                eventRaised = true;
            };
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(50);
                testObj.TestAccessorProperty = 12;
            }));

            thread.Start();
            thread.Join();

            Assert.IsTrue(eventRaised, "Expected event was not raised");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is executed, when the property changes.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessorAndCallback_Changed_ExecutesCallback()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            bool callbackExecuted = false;
            testObj.SetCallback((oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            testObj.TestAccessorPropertyWithCallback = new object();

            Assert.IsTrue(callbackExecuted, "Callback was not executed");
        }

        /// <summary>
        /// Ensures that the provided propertyChangedCallback parameter is not executed,
        /// when the property does not change.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessorAndCallback_NotChanged_DoesNotExecuteCallback()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            var objValue = new object();
            testObj.TestAccessorPropertyWithCallback = objValue;
            bool callbackExecuted = false;
            testObj.SetCallback((oldVal, newVal) =>
            {
                callbackExecuted = true;
            });

            testObj.TestAccessorPropertyWithCallback = objValue;

            Assert.IsFalse(callbackExecuted, "Callback was executed, but shouldn't have.");
        }

        /// <summary>
        /// Ensures that when the propertyChangedCallback parameter is executed, it is provided with the
        /// correct old/new property values.
        /// </summary>
        [TestMethod]
        public void Set_WithAccessorAndCallback_Changed_ProvidesCorrectOldAndNewValues()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            object expectedOld = new object();
            object expectedNew = new object();
            object actualOld = null;
            object actualNew = null;
            testObj.TestAccessorPropertyWithCallback = expectedOld;
            testObj.SetCallback((oldVal, newVal) =>
            {
                actualOld = oldVal;
                actualNew = newVal;
            });

            testObj.TestAccessorPropertyWithCallback = expectedNew;

            Assert.AreEqual(expectedOld, actualOld, "'Old' values do not match.");
            Assert.AreEqual(expectedNew, actualNew, "'New' values do not match.");
        }

        #endregion Set (with Accessor<T>) Tests

        #region Dependency Mapping Tests

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
            testObj.ReactionProperty = onDependencyChanged.Object;
            testObj.DependencyProperty = new object();

            Assert.IsTrue(callbackExecuted, "Expected OnDependencyChanged callback to be executed.");
        }

        /// <summary>
        /// Ensures that the Dependency Mapping facilities correctly
        /// provides dependent properties during the NotifyPropertyChanged methods,
        /// when the <see cref="DependsOnAttribute"/> is used for dependency mapping.
        /// </summary>
        [TestMethod]
        public void DependsOnAttribute_WhenPropertyHasDirectDependants_RaisesPropertyChangedForDependants()
        {
            bool eventRaised = false;
            var testObj = new ObservableTestObject();
            testObj.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "DirectlyDependantProperty")
                    eventRaised = true;
            };
            testObj.DependencyProperty = new object();

            Assert.IsTrue(eventRaised, "Expected PropertyChanged event to be raised for directly-dependent property.");
        }

        /// <summary>
        /// Ensures that the Dependency Mapping facilities correctly
        /// provides dependent properties during the NotifyPropertyChanged methods,
        /// when the <see cref="DependsOnAttribute"/> is used for dependency mapping.
        /// </summary>
        [TestMethod]
        public void DependsOnAttribute_WhenPropertyHasIndirectDependants_RaisesPropertyChangedForDependants()
        {
            bool eventRaised = false;
            var testObj = new ObservableTestObject();
            testObj.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IndirectlyDependantProperty")
                    eventRaised = true;
            };
            testObj.DependencyProperty = new object();

            Assert.IsTrue(eventRaised, "Expected PropertyChanged event to be raised for directly-dependent property.");
        }

        /// <summary>
        /// Ensures that the Dependency Mapping facilities correctly
        /// provides dependent properties during the NotifyPropertyChanged methods,
        /// when the Fluent mapping syntax is used for dependency mapping.
        /// </summary>
        [TestMethod]
        public void FluentDependencySyntax_WhenPropertyHasDirectDependants_RaisesPropertyChangedForDependants()
        {
            bool eventRaised = false;
            var testObj = new ObservableTestObject();
            testObj.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "DirectlyDependantFluentProperty")
                    eventRaised = true;
            };

            testObj.Property("DirectlyDependantFluentProperty").DependsOn("DependencyProperty");
            testObj.DependencyProperty = new object();

            Assert.IsTrue(eventRaised, "Expected PropertyChanged event to be raised for directly-dependent property.");
        }

        /// <summary>
        /// Ensures that the Dependency Mapping facilities correctly
        /// provides dependent properties during the NotifyPropertyChanged methods,
        /// when the Fluent mapping syntax is used for dependency mapping.
        /// </summary>
        [TestMethod]
        public void FluentDependencySyntax_WhenPropertyHasIndirectDependants_CallsOnPropertyChangedForDependants()
        {
            bool eventRaised = false;
            var testObj = new ObservableTestObject();
            testObj.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "IndirectlyDependantFluentProperty")
                    eventRaised = true;
            };

            testObj.Property("IndirectlyDependantFluentProperty").DependsOn("DirectlyDependantFluentProperty");
            testObj.Property("DirectlyDependantFluentProperty").DependsOn("DependencyProperty");
            testObj.DependencyProperty = new object();

            Assert.IsTrue(eventRaised, "Expected PropertyChanged event to be raised for indirectly-dependent property.");
        }

        #endregion
    }
}