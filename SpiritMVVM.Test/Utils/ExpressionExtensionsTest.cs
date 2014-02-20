using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Utils;

namespace SpiritMVVM.Test.Utils
{
    /// <summary>
    /// Unit tests for methods contained within the static <see cref="ExpressionExtensions"/> class.
    /// </summary>
    [TestClass]
    public class ExpressionExtensionsTest
    {
        /// <summary>
        /// Plain data class which contains a single property with a get/set accessor.
        /// </summary>
        public class TestClass
        {
            /// <summary>
            /// Get or Set the MyProperty string value.
            /// </summary>
            public string MyProperty { get; set; }

            /// <summary>
            /// Get or Set the static property
            /// </summary>
            public static string MyStaticProperty { get; set; }
        }

        /// <summary>
        /// Ensure that the <see cref="ExpressionExtensions.GetParentInstance"/> method returns the parent object of the selected property.
        /// </summary>
        [TestMethod]
        public void GetParentInstance_WithInstanceBasedPath_ReturnsParentObjectOfSelectedProperty()
        {
            var testInstance = CreateTestInstance();
            Expression<Func<string>> expression = (() => testInstance.MyProperty);
            var memberExpression = (MemberExpression)expression.Body;
            var parent = memberExpression.GetParentInstance();

            Assert.AreEqual(testInstance, parent, "Expected 'parent' and 'testInstance' to reference the same object, but they did not.");
        }

        /// <summary>
        /// Ensure that the <see cref="ExpressionExtensions.GetParentInstance"/> method returns the parent object of the selected property.
        /// </summary>
        [TestMethod]
        public void GetParentInstance_WithStaticMemberInPath_ThrowsException()
        {
            var testInstance = CreateTestInstance();
            Expression<Func<string>> expression = (() => TestClass.MyStaticProperty);
            var memberExpression = (MemberExpression)expression.Body;

            try
            {
                var parent = memberExpression.GetParentInstance();
                Assert.Fail("Expected an ArgumentException that was not thrown.");
            }
            catch (ArgumentException)
            {
                //Pass
            }
        }

        /// <summary>
        /// Ensure that the <see cref="ExpressionExtensions.GetAccessorForProperty"/> method returns an accessor which can successfully "Get" the property's value.
        /// </summary>
        [TestMethod]
        public void GetAccessorForProperty_GetAccessor_ReturnsCorrectValue()
        {
            var testInstance = CreateTestInstance();
            Expression<Func<string>> expression = (() => testInstance.MyProperty);
            var accessor = expression.GetAccessorForProperty();

            Assert.AreEqual(accessor.Value, testInstance.MyProperty, "Expected accessor to return value of 'MyProperty,' but it did not.");
        }

        /// <summary>
        /// Create an instance of the <see cref="TestClass"/> object with a random string value.
        /// </summary>
        /// <returns>Returns a random new Test object.</returns>
        public TestClass CreateTestInstance()
        {
            return new TestClass()
            {
                MyProperty = Guid.NewGuid().ToString()
            };
        }
    }
}
