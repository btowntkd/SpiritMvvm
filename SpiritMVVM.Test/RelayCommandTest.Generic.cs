using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiritMVVM.Test
{
	[TestClass]
	public class RelayCommandTestGeneric
	{
        [TestMethod]
        public void Constructor_NullAction_ThrowsException()
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

        [TestMethod]
        public void Constructor_WithAction_Success()
        {
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }));
        }

        [TestMethod]
        public void CanExecute_NullPredicate_ReturnsTrueAlways()
        {
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }));
            Assert.IsTrue(command.CanExecute(null));
        }

        [TestMethod]
        public void CanExecute_WithPredicate_ExecutesPredicate()
        {
            bool actionExecuted = false;
            RelayCommand<object> command = new RelayCommand<object>(new Action<object>((x) => { /* Do nothing */ }),
                new Func<object, bool>((x) => { actionExecuted = true; return true; }));

            command.CanExecute(null);
            Assert.IsTrue(actionExecuted, "Expected predicate to be executed.");
        }

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
