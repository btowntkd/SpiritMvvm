using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace SpiritMVVM.Test
{
    [TestClass]
    public class ObservableObjectTest
    {
        public class ObservableTestObject : ObservableObject
        {
            public void SetPropertyHelper(IPropertyNotifier helper)
            {
                PropertyNotifier = helper;
            }

            public IPropertyNotifier GetPropertyHelper()
            {
                return PropertyNotifier;
            }

            private int _testPropertyWithRef = 0;
            public int TestPropertyWithRef
            {
                get { return _testPropertyWithRef; }
                set { Set(ref _testPropertyWithRef, value); }
            }

        }

        [TestMethod]
        public void SetPropertyNotifier_NullValue_ThrowsException()
        {
            try
            {
                ObservableTestObject testObj = new ObservableTestObject();
                testObj.SetPropertyHelper(null);
                Assert.Fail("Expected exception");
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not the expected type");
            }
        }

        [TestMethod]
        public void Set_CallsPropertyNotifier()
        {
            Mock<IPropertyNotifier> mockNotifier = new Mock<IPropertyNotifier>();

            ObservableTestObject testObj = new ObservableTestObject();
            testObj.SetPropertyHelper(mockNotifier.Object);
            testObj.TestPropertyWithRef = 12;

            var intRef = It.IsAny<int>();
            mockNotifier.Verify((x) => 
                x.SetProperty<int>(
                    ref intRef, 
                    It.IsAny<int>(), 
                    It.IsAny<Action<int, int>>(), 
                    It.IsAny<string>()), 
                Moq.Times.Once());
        }
    }
}
