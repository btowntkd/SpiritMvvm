using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SpiritMVVM.Test
{
    /// <summary>
    /// Unit tests for the <see cref="RelayCommand{T}"/> class.
    /// </summary>
    [TestClass]
    public class RelayCommandTestGeneric
    {
        /// <summary>
        /// Ensures that the constructor will fail with a <see cref="ArgumentNullException"/>
        /// when an Execute Action is not provided.
        /// </summary>
        [TestMethod]
        public void Constructor_NullAction_ThrowsArgumentNullException()
        {
            try
            {
                RelayCommand<object> command = new RelayCommand<object>(null);
                Assert.Fail("Expected an exception.");
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not the expected type.");
            }
        }

        /// <summary>
        /// Ensures that the constructor does not fail, when given a non-null <see cref="Action"/>.
        /// </summary>
        [TestMethod]
        public void Constructor_WithAction_Success()
        {
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }));
        }

        /// <summary>
        /// Ensures that the CanExecute method will default to "True" when no predicate is provided in the constructor.
        /// </summary>
        [TestMethod]
        public void CanExecute_NullPredicate_ReturnsTrueAlways()
        {
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }));
            Assert.IsTrue(command.CanExecute(null));
        }

        /// <summary>
        /// Ensures that the CanExecute method will execute the predicate (provided in the constructor).
        /// </summary>
        [TestMethod]
        public void CanExecute_WithPredicate_ExecutesPredicate()
        {
            bool actionExecuted = false;
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }),
                new Func<object, bool>((x) => { actionExecuted = true; return true; }));

            command.CanExecute(null);
            Assert.IsTrue(actionExecuted, "Expected predicate to be executed.");
        }

        /// <summary>
        /// Ensures that the predicate (provided in the constructor) will recieve the parameter passed to the
        /// CanExecute method.
        /// </summary>
        [TestMethod]
        public void CanExecute_WithPredicate_ReceivesParameter()
        {
            object parameter = new object();
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }),
                new Func<object, bool>((x) =>
                {
                    Assert.AreEqual(parameter, x, "Expected parameter and predicate argument to be equal");
                    return true;
                }));

            command.CanExecute(parameter);
        }

        /// <summary>
        /// Ensures that the CanExecute method will return whatever value is returned by the predicate.
        /// </summary>
        [TestMethod]
        public void CanExecute_WithPredicate_ReturnsPredicateValue()
        {
            bool expectedValue = false;
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }),
                new Func<object, bool>((x) => { return expectedValue; }));

            expectedValue = false;
            Assert.AreEqual(expectedValue, command.CanExecute(null), "Returned value did not match expected value.");

            expectedValue = true;
            Assert.AreEqual(expectedValue, command.CanExecute(null), "Returned value did not match expected value.");
        }

        /// <summary>
        /// Ensures that the Execute method will execute the Action provided in the constructor.
        /// </summary>
        [TestMethod]
        public void Execute_WithAction_ExecutesDelegateAction()
        {
            bool actionExecuted = false;
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) =>
            {
                actionExecuted = true;
            }));

            command.Execute(null);
            Assert.IsTrue(actionExecuted, "Expected delegate action to be executed.");
        }

        /// <summary>
        /// Ensures that the Execute method will pass the parameter to the Action provided in the constructor.
        /// </summary>
        [TestMethod]
        public void Execute_WithAction_ReceivesParameter()
        {
            object parameter = new object();
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) =>
            {
                Assert.AreEqual(parameter, x, "Expected parameter and action argument to be equal");
            }));

            command.Execute(parameter);
        }
    }
}