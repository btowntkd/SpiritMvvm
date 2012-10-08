using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiritMVVM.Test
{
    [TestClass]
    public class RelayCommandTest
    {
        [TestMethod]
        public void Constructor_NullAction_ThrowsException()
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

        [TestMethod]
        public void Constructor_WithAction_Success()
        {
            RelayCommand command = new RelayCommand(new Action(() => { /* Do nothing */ }));
        }

        [TestMethod]
        public void CanExecute_NullPredicate_ReturnsTrueAlways()
        {
            RelayCommand command = new RelayCommand(new Action(() => { /* Do nothing */ }));
            Assert.IsTrue(command.CanExecute(null));
        }

        [TestMethod]
        public void CanExecute_WithPredicate_ExecutesPredicate()
        {
            bool actionExecuted = false;
            RelayCommand command = new RelayCommand(new Action(() => { /* Do nothing */ }),
                new Func<bool>(() => { actionExecuted = true; return true; }));

            command.CanExecute(null);
            Assert.IsTrue(actionExecuted, "Expected predicate to be executed.");
        }

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
