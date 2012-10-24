using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiritMVVM.Test
{
    /// <summary>
    /// Unit tests for the <see cref="RelayCommand"/> class.
    /// </summary>
    [TestClass]
    public class RelayCommandTest
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
                RelayCommand command = new RelayCommand(null);
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
            RelayCommand command = new RelayCommand(new Action(() => { /* Do nothing */ }));
        }

        /// <summary>
        /// Ensures that the CanExecute method will default to "True" when no predicate is provided in the constructor.
        /// </summary>
        [TestMethod]
        public void CanExecute_NullPredicate_ReturnsTrueAlways()
        {
            RelayCommand command = new RelayCommand(new Action(() => { /* Do nothing */ }));
            Assert.IsTrue(command.CanExecute(null));
        }

        /// <summary>
        /// Ensures that the CanExecute method will execute the predicate (provided in the constructor).
        /// </summary>
        [TestMethod]
        public void CanExecute_WithPredicate_ExecutesPredicate()
        {
            bool actionExecuted = false;
            RelayCommand command = new RelayCommand(new Action(() => { /* Do nothing */ }),
                new Func<bool>(() => { actionExecuted = true; return true; }));

            command.CanExecute(null);
            Assert.IsTrue(actionExecuted, "Expected predicate to be executed.");
        }

        /// <summary>
        /// Ensures that the CanExecute method will return whatever value is returned by the predicate.
        /// </summary>
        [TestMethod]
        public void CanExecute_WithPredicate_ReturnsPredicateValue()
        {
            bool expectedValue = false;
            RelayCommand command = new RelayCommand(new Action(() => { /* Do nothing */ }),
                new Func<bool>(() => { return expectedValue; }));

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
            RelayCommand command = new RelayCommand(new Action(() => 
            {
                actionExecuted = true;
            }));

            command.Execute(null);
            Assert.IsTrue(actionExecuted, "Expected delegate action to be executed.");
        }
    }
}
