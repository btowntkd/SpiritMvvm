using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpiritMVVM.Test
{
    internal class SimpleValidatableObject : ValidatableObject
    {
        [Required(ErrorMessage = "string is required")]
        public string TestString { get; set; }

        [Range(10, 20, ErrorMessage = "int must be between 10 and 20")]
        public int TestInt { get; set; }
    }

    /// <summary>
    /// Unit tests for the <see cref="ObjectValidator{T}"/> class.
    /// </summary>
    [TestClass]
    public class ObjectValidatorTest
    {
        /// <summary>
        /// Ensures that the ObjectValidator will correctly add errors to a target
        /// <see cref="IValidatableObject"/> when the target object contains validation errors
        /// (according to its Data Annotations).
        /// </summary>
        [TestMethod]
        public void Validate_WhenInvalid_AddsErrors()
        {
            //Arrange
            var validator = new ObjectValidator<SimpleValidatableObject>();
            var target = new SimpleValidatableObject();

            //Act
            Assert.IsFalse(target.HasErrors, "Did not expect target object to have errors, yet");
            validator.Validate(target);

            //Assert
            var errors_TestInt = target.GetErrors("TestInt").Cast<string>();
            var errors_TestString = target.GetErrors("TestString").Cast<string>();
            Assert.AreEqual(errors_TestInt.Count(), 1, "Expected TestInt to be flagged with errors");
            Assert.AreEqual(errors_TestString.Count(), 1, "Expected TestString to be flagged with errors");
        }

        /// <summary>
        /// Ensures that the ObjectValidator will correctly clear errors from a target
        /// <see cref="IValidatableObject"/> when the target object contains no validation errors
        /// (according to its Data Annotations).
        /// </summary>
        [TestMethod]
        public void Validate_WhenValid_ClearsErrors()
        {
            //Arrange
            var validator = new ObjectValidator<SimpleValidatableObject>();
            var target = new SimpleValidatableObject();

            //Act
            validator.Validate(target);
            target.TestInt = 11;
            target.TestString = "Hello";
            validator.Validate(target);

            //Assert
            var errors_TestInt = target.GetErrors("TestInt").Cast<string>();
            var errors_TestString = target.GetErrors("TestString").Cast<string>();
            Assert.AreEqual(errors_TestInt.Count(), 0, "Expected TestInt to NOT be flagged with errors");
            Assert.AreEqual(errors_TestString.Count(), 0, "Expected TestString to NOT be flagged with errors");
        }

        /// <summary>
        /// Ensures that the ObjectValidator will correctly add errors to a target
        /// <see cref="IValidatableObject"/> when the target object contains validation errors
        /// (according to its Data Annotations).
        /// </summary>
        [TestMethod]
        public void ValidateProperty_WhenInvalid_AddsErrorsToPropertyOnly()
        {
            //Arrange
            var validator = new ObjectValidator<SimpleValidatableObject>();
            var target = new SimpleValidatableObject();

            //Act
            Assert.IsFalse(target.HasErrors, "Did not expect target object to have errors, yet");
            validator.ValidateProperty(target, "TestInt");

            //Assert
            var errors_TestInt = target.GetErrors("TestInt").Cast<string>();
            var errors_TestString = target.GetErrors("TestString").Cast<string>();
            Assert.AreEqual(errors_TestInt.Count(), 1, "Expected TestInt to be flagged with errors");
            Assert.AreEqual(errors_TestString.Count(), 0, "Expected TestString to NOT be flagged with errors");
        }

        /// <summary>
        /// Ensures that the ObjectValidator will correctly clear errors from a target
        /// <see cref="IValidatableObject"/> when the target object contains no validation errors
        /// (according to its Data Annotations).
        /// </summary>
        [TestMethod]
        public void ValidateProperty_WhenValid_ClearsErrorsFromPropertyOnly()
        {
            //Arrange
            var validator = new ObjectValidator<SimpleValidatableObject>();
            var target = new SimpleValidatableObject();

            //Act
            validator.Validate(target);
            target.TestInt = 11;
            target.TestString = "Hello";
            validator.ValidateProperty(target, "TestInt");

            //Assert
            var errors_TestInt = target.GetErrors("TestInt").Cast<string>();
            var errors_TestString = target.GetErrors("TestString").Cast<string>();
            Assert.AreEqual(errors_TestInt.Count(), 0, "Expected TestInt to NOT be flagged with errors");
            Assert.AreEqual(errors_TestString.Count(), 1, "Expected TestString to be flagged with errors");
        }
    }
}
