using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using SpiritMVVM.Messaging;
using SpiritMVVM.ViewModels;

namespace SpiritMVVM.Test.ViewModels
{
    /// <summary>
    /// Unit tests for the <see cref="ViewModelBase"/> class.
    /// </summary>
    [TestClass]
    public class ViewModelBaseTest
    {
        /// <summary>
        /// Ensures that the default constructor will assign the default <see cref="ViewModelBase.Messenger"/> property.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_AssignsMessengerDefault()
        {
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();
            Assert.AreEqual(viewModelMock.Object.Messenger, Messenger.Default, "Expected Messenger.Default to be the default Messenger value.");
        }

        /// <summary>
        /// Ensures that the constructor with a messenger parameter will assign the provided
        /// value to the <see cref="ViewModelBase.Messenger"/> property.
        /// </summary>
        [TestMethod]
        public void Constructor_WithMessenger_AssignsMessengerValue()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>(new object[]{ messenger });
            Assert.AreEqual(viewModelMock.Object.Messenger, messenger, "Expected Messenger to match the provided constructor argument.");
        }

        /// <summary>
        /// Ensures that the constructor will not result in execution of the <see cref="ViewModelBase.OnMessengerChanged"/> method.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_DoesNotCallOnMessengerChanged()
        {
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();
            viewModelMock.Protected().Setup("OnMessengerChanged", ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
            viewModelMock.Protected().Verify("OnMessengerChanged", Times.Never(), ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
        }

        /// <summary>
        /// Ensures that the constructor will not result in execution of the <see cref="ViewModelBase.OnMessengerChanged"/> method.
        /// </summary>
        [TestMethod]
        public void Constructor_WithMessenger_DoesNotCallOnMessengerChanged()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>(new object[] { messenger });
            viewModelMock.Protected().Setup("OnMessengerChanged", ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
            viewModelMock.Protected().Verify("OnMessengerChanged", Times.Never(), ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
        }

        /// <summary>
        /// Ensures that calling Get/Set on the <see cref="Messenger"/> property will assign and return the correct values.
        /// </summary>
        [TestMethod]
        public void GetSetMessenger_WithValue_AssignsAndReturnsMessengerValue()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();

            Assert.AreNotEqual(viewModelMock.Object.Messenger, messenger, "Did not expect messenger values to match yet.");
            viewModelMock.Object.Messenger = messenger;
            Assert.AreEqual(viewModelMock.Object.Messenger, messenger, "Expected messenger values to match.");
        }

        /// <summary>
        /// Ensures that assigning a new value to the <see cref="ViewModelBase.Messenger"/> property will result 
        /// in execution of the <see cref="ViewModelBase.OnMessengerChanged"/> method.
        /// </summary>
        [TestMethod]
        public void SetMessenger_WithValue_CallsOnMessengerChanged()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();

            viewModelMock.Object.Messenger = messenger;
            viewModelMock.Protected().Setup("OnMessengerChanged", ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
            viewModelMock.Protected().Verify("OnMessengerChanged", Times.AtLeastOnce(), ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
        }
    }
}
