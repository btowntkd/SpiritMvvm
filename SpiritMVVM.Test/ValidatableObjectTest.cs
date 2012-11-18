using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiritMVVM.Test
{
    /// <summary>
    /// Unit tests for the <see cref="ValidatableObject"/> class.
    /// </summary>
    [TestClass]
    public class ValidatableObjectTest
    {
        /// <summary>
        /// Ensures that we can successfully create a new ValidatableObject
        /// </summary>
        [TestMethod]
        public void Constructor_Success()
        {
            ValidatableObject obj = new ValidatableObject();
        }

        /// <summary>
        /// Ensures that adding an error will raise the ErrorsChanged event.
        /// </summary>
        [TestMethod]
        public void AddError_WithError_RaisesErrorsChangedEventWithPropertyName()
        {
            bool eventRaised = false;
            string expectedPropertyName = "TestProperty";
            string receivedPropertyName = string.Empty;
            var testObj = new ValidatableObject();

            testObj.ErrorsChanged += (sender, args) =>
            {
                eventRaised = true;
                receivedPropertyName = args.PropertyName;
            };

            testObj.AddError(expectedPropertyName, "BlahBlahBlah");

            Assert.IsTrue(eventRaised, "The ErrorsChanged event was not raised.");
            Assert.AreEqual(expectedPropertyName, receivedPropertyName, 
                "The received PropertyName event argument was not equal"
                + " to the property name specified in AddError.");
        }

        /// <summary>
        /// Ensures that AddError will add an error to the list of errors for
        /// the given property.
        /// </summary>
        [TestMethod]
        public void AddError_WithError_IsAddedToErrorsList()
        {
            const string propName = "MyProperty";
            var testObj = new ValidatableObject();

            var beforeAddedError = testObj.GetErrors(propName).OfType<object>().ToList();
            //var errorToAdd = new object();
            var errorToAdd = "BLAH";
            testObj.AddError(propName, errorToAdd);
            var afterAddedError = testObj.GetErrors(propName).OfType<object>().ToList();

            Assert.IsTrue(afterAddedError.Count == (beforeAddedError.Count + 1), "Expected 1 additional error.");
            Assert.IsTrue(afterAddedError.Contains(errorToAdd), "Could not find expected error instance in list.");
        }

        /// <summary>
        /// Ensures the the HasErrors property will return true when there are errors.
        /// </summary>
        [TestMethod]
        public void HasErrors_WithErrors_ReturnsTrue()
        {
            var testObj = new ValidatableObject();
            const string propName = "MyProperty";
            object error = new object();
            testObj.AddError(propName, error);

            Assert.IsTrue(testObj.HasErrors, "Expected HasErrors to return true.");
        }

        /// <summary>
        /// Ensures that the HasErrors property will return false when there are no errors.
        /// </summary>
        [TestMethod]
        public void HasErrors_NoErrors_ReturnsFalse()
        {
            var testObj = new ValidatableObject();
            Assert.IsFalse(testObj.HasErrors, "Expected HasErrors to return false");
        }

        /// <summary>
        /// Ensures the the IsValid property will return false when there are errors.
        /// </summary>
        [TestMethod]
        public void IsValid_WithErrors_ReturnsFalse()
        {
            var testObj = new ValidatableObject();
            const string propName = "MyProperty";
            object error = new object();
            testObj.AddError(propName, error);

            Assert.IsFalse(testObj.IsValid, "Expected IsValid to return false.");
        }

        /// <summary>
        /// Ensures that the IsValid property will return true when there are no errors.
        /// </summary>
        [TestMethod]
        public void IsValid_NoErrors_ReturnsTrue()
        {
            var testObj = new ValidatableObject();
            Assert.IsTrue(testObj.IsValid, "Expected IsValid to return true");
        }

        /// <summary>
        /// Ensures that ClearErrors will remove all current errors for the 
        /// specified property name, and *ONLY* for that property.
        /// </summary>
        [TestMethod]
        public void ClearErrors_AfterErrorsAdded_RemovesAllErrorsOnlyForSpecifiedProperty()
        {
            var testObj = new ValidatableObject();

            const string firstProperty = "MyProperty1";
            const string secondProperty = "MyProperty2";
            var firstError = "BLAH1";
            var secondError = "BLAH2";
            
            //Add 4 errors total - 2 errors for each of 2 properties
            testObj.AddError(firstProperty, firstError);
            testObj.AddError(firstProperty, secondError);
            testObj.AddError(secondProperty, firstError);
            testObj.AddError(secondProperty, secondError);

            //Clear the errors for the 1st property only
            testObj.ClearErrors(firstProperty);

            //Get the list of current errors for both properties
            var firstPropertyErrors = testObj.GetErrors(firstProperty).OfType<object>().ToList();
            var secondPropertyErrors = testObj.GetErrors(secondProperty).OfType<object>().ToList();

            Assert.IsTrue(firstPropertyErrors.Count == 0, "Did not expect errors to exist on first property.");
            Assert.IsTrue(secondPropertyErrors.Count > 0, "Expected errors to exist on second property.");
        }

        /// <summary>
        /// Ensures that ClearErrors will raise the ErrorsChanged
        /// event, if the property had errors to begin with.
        /// </summary>
        [TestMethod]
        public void ClearErrors_AfterErrorsAdded_RaisesErrorsChangedEvent()
        {
            bool eventRaised = false;
            string expectedPropertyName = "TestProperty";
            string receivedPropertyName = string.Empty;
            var testObj = new ValidatableObject();
            testObj.AddError(expectedPropertyName, "BlahBlahBlah");

            testObj.ErrorsChanged += (sender, args) =>
            {
                eventRaised = true;
                receivedPropertyName = args.PropertyName;
            };

            testObj.ClearErrors(expectedPropertyName);

            Assert.IsTrue(eventRaised, "The ErrorsChanged event was not raised.");
            Assert.AreEqual(expectedPropertyName, receivedPropertyName,
                "The received PropertyName event argument was not equal"
                + " to the property name specified in AddError.");
        }

        /// <summary>
        /// Ensures that ClearErrors will NOT raise the ErrorsChanged
        /// event, if the property did not have errors to begin with.
        /// </summary>
        [TestMethod]
        public void ClearErrors_WithoutErrorsAdded_DoesNotRaiseErrorsChangedEvent()
        {
            bool eventRaised = false;
            var testObj = new ValidatableObject();
            testObj.ErrorsChanged += (sender, args) =>
            {
                eventRaised = true;
            };
            testObj.ClearErrors("SomeRandomProperty");

            Assert.IsFalse(eventRaised, "The ErrorsChanged event was raised, but should not have.");
        }

        /// <summary>
        /// Ensures that GetErrors will return all current errors for the 
        /// specified property name, and *ONLY* for that property.
        /// </summary>
        [TestMethod]
        public void GetErrors_WithErrorsAdded_ReturnsAllErrorsForSpecifiedProperty()
        {
            var testObj = new ValidatableObject();

            const string firstProperty = "MyProperty1";
            const string secondProperty = "MyProperty2";
            var firstError = "BLAH1";
            var secondError = "BLAH2";
            var thirdError = "BLAH3";
            var fourthError = "BLAH4";

            //Add 4 errors total - 2 errors for each of 2 properties
            testObj.AddError(firstProperty, firstError);
            testObj.AddError(firstProperty, secondError);
            testObj.AddError(secondProperty, thirdError);
            testObj.AddError(secondProperty, fourthError);

            //Get the list of current errors for both properties
            var firstPropertyErrors = testObj.GetErrors(firstProperty).OfType<object>().ToList();
            var secondPropertyErrors = testObj.GetErrors(secondProperty).OfType<object>().ToList();

            //Ensure that the returned list of errors contains only the errors
            //added to the MyProperty1 property key.
            Assert.IsTrue(firstPropertyErrors.Count == 2, "Error count was not the expected value.");
            Assert.IsTrue(firstPropertyErrors.Contains(firstError), "Did not find expected error");
            Assert.IsTrue(firstPropertyErrors.Contains(secondError), "Did not find expected error");
        }
    }
}
