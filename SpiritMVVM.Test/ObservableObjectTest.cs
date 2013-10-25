using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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

            private object _testFluentDependantProperty = null;

            public object TestFluentDependantProperty
            {
                get { return _testFluentDependantProperty; }
                set { Set(ref _testFluentDependantProperty, value); }
            }
        }

        /// <summary>
        /// Ensures that a property can be changed from a different thread.
        /// </summary>
        [TestMethod]
        public void Set_FromDifferentThread_CallsPropertyNotifier()
        {
            ObservableTestObject testObj = new ObservableTestObject();
            bool eventRaised = false;
            testObj.PropertyChanged += (obj, args) =>
            {
                eventRaised = true;
            };
            Task.Factory.StartNew(() => testObj.TestPropertyWithRef = 12, TaskCreationOptions.LongRunning).Wait();

            Assert.IsTrue(eventRaised, "Expected event was not raised");
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

        /// <summary>
        /// Ensures that the Dependency Mapping facilities correctly
        /// provides dependent properties during the NotifyPropertyChanged methods,
        /// when the <see cref="DependsOnAttribute"/> is used for dependency mapping.
        /// </summary>
        [TestMethod]
        public void DependsOnAttribute_WhenPropertyHasDependants_CallsOnPropertyChangedForDependants()
        {
            bool eventRaised = false;
            var testObj = new ObservableTestObject();
            testObj.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TestDependantProperty")
                    eventRaised = true;
            };
            testObj.TestPropertyWithRef = 12;

            Assert.IsTrue(eventRaised, "Expected PropertyChanged event to be raised for directly-dependent property.");
        }

        /// <summary>
        /// Ensures that the Dependency Mapping facilities correctly
        /// provides dependent properties during the NotifyPropertyChanged methods,
        /// when the Fluent mapping syntax is used for dependency mapping.
        /// </summary>
        [TestMethod]
        public void FluentDependencySyntax_WhenPropertyHasDirectDependants_CallsOnPropertyChangedForDependants()
        {
            bool eventRaised = false;
            var testObj = new ObservableTestObject();
            testObj.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "TestFluentDependantProperty")
                    eventRaised = true;
            };

            testObj.Property("TestFluentDependantProperty").DependsOn("TestPropertyWithRef");
            testObj.TestPropertyWithRef = 12;

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
                if (args.PropertyName == "TestFluentDependantProperty")
                    eventRaised = true;
            };

            testObj.Property("TestFluentDependantProperty").DependsOn("TestDependantProperty");
            testObj.TestPropertyWithRef = 12;

            Assert.IsTrue(eventRaised, "Expected PropertyChanged event to be raised for indirectly-dependent property.");
        }
    }
}