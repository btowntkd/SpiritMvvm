using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using SpiritMVVM.Messaging;
using SpiritMVVM.ViewModels;

namespace SpiritMVVM.Test.ViewModels
{
    /// <summary>
    /// Unit tests for the <see cref="ViewModelBase{T}"/> class.
    /// </summary>
	[TestClass]
	public class ViewModelBaseTestGeneric
	{
        /// <summary>
        /// Ensures that the default constructor will assign the expected default values to 
        /// the Model and Messenger properties.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_AssignsDefaultModelAndMessenger()
        {
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>();
            Assert.AreEqual(viewModelMock.Object.Model, default(object), "Expected Model to be the default Model value.");
            Assert.AreEqual(viewModelMock.Object.Messenger, Messenger.Default, "Expected Messenger.Default to be the default Messenger value.");
        }

        /// <summary>
        /// Ensures that the constructor with a messenger parameter will assign the expected default value to 
        /// the Model property, and assign the provided value to the Messenger property.
        /// </summary>
        [TestMethod]
        public void Constructor_WithMessenger_AssignsDefaultModelAndGivenMessengerValue()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>(new object[] { messenger });
            Assert.AreEqual(viewModelMock.Object.Model, default(object), "Expected Model to be the default Model value.");
            Assert.AreEqual(viewModelMock.Object.Messenger, messenger, "Expected Messenger to match the provided constructor argument.");
        }

        /// <summary>
        /// Ensures that the constructor with a model parameter will assign the expected default value to 
        /// the Messenger property, and assign the provided value to the Model property.
        /// </summary>
        [TestMethod]
        public void Constructor_WithModel_AssignsGivenModelValueAndDefaultMessenger()
        {
            object model = new object();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>(new object[] { model });
            Assert.AreEqual(viewModelMock.Object.Model, model, "Expected Model to match the provided constructor argument.");
            Assert.AreEqual(viewModelMock.Object.Messenger, Messenger.Default, "Expected Messenger.Default to be the default Messenger value.");
        }

        /// <summary>
        /// Ensures that the constructor with model and messenger parameters will assign the
        /// provided values to the Model and Messenger properties.
        /// </summary>
        [TestMethod]
        public void Constructor_WithModelAndMessenger_AssignsGivenModelAndMessengerValues()
        {
            object model = new object();
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>(new object[] { model, messenger });
            Assert.AreEqual(viewModelMock.Object.Model, model, "Expected Model to match the provided constructor argument.");
            Assert.AreEqual(viewModelMock.Object.Messenger, messenger, "Expected Messenger to match the provided constructor argument.");
        }

        /// <summary>
        /// Ensures that the constructor will not result in execution of the OnModelChanged method.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_DoesNotCallOnModelChanged()
        {
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>();
            viewModelMock.Protected().Setup("OnModelChanged", ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
            viewModelMock.Protected().Verify("OnModelChanged", Times.Never(), ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
        }

        /// <summary>
        /// Ensures that the constructor will not result in execution of the OnModelChanged method.
        /// </summary>
        [TestMethod]
        public void Constructor_WithModel_DoesNotCallOnModelChanged()
        {
            object model = new object();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>(new object[] { model });
            viewModelMock.Protected().Setup("OnModelChanged", ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
            viewModelMock.Protected().Verify("OnModelChanged", Times.Never(), ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
        }

        /// <summary>
        /// Ensures that the constructor will not result in execution of the OnModelChanged method.
        /// </summary>
        [TestMethod]
        public void Constructor_WithMessenger_DoesNotCallOnModelChanged()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>(new object[] { messenger });
            viewModelMock.Protected().Setup("OnModelChanged", ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
            viewModelMock.Protected().Verify("OnModelChanged", Times.Never(), ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
        }

        /// <summary>
        /// Ensures that the constructor will not result in execution of the OnModelChanged method.
        /// </summary>
        [TestMethod]
        public void Constructor_WithModelAndMessenger_DoesNotCallOnModelChanged()
        {
            object model = new object();
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>(new object[] { model, messenger });
            viewModelMock.Protected().Setup("OnModelChanged", ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
            viewModelMock.Protected().Verify("OnModelChanged", Times.Never(), ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
        }

        /// <summary>
        /// Ensures that the Get/Set accessors are able to assign and return the expected value.
        /// </summary>
        [TestMethod]
        public void GetSetModel_WithValue_AssignsAndReturnsModelValue()
        {
            object model = new object();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>();

            Assert.AreNotEqual(viewModelMock.Object.Model, model, "Did not expect model values to match yet.");
            viewModelMock.Object.Model = model;
            Assert.AreEqual(viewModelMock.Object.Model, model, "Expected model values to match.");
        }

        /// <summary>
        /// Ensures that assigning a new value to the Model property will result 
        /// in execution of the <see cref="ViewModelBase{T}.OnModelChanged"/> method.
        /// </summary>
        [TestMethod]
        public void SetModel_WithValue_CallsOnMessengerChanged()
        {
            object model = new object();
            Mock<ViewModelBase<object>> viewModelMock = new Mock<ViewModelBase<object>>();

            viewModelMock.Object.Model = model;
            viewModelMock.Protected().Setup("OnModelChanged", ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
            viewModelMock.Protected().Verify("OnModelChanged", Times.Once(), ItExpr.IsAny<object>(), ItExpr.IsAny<object>());
        }
	}
}
