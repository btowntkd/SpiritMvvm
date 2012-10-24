using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using SpiritMVVM.Messaging;
using SpiritMVVM.ViewModels;

namespace SpiritMVVM.Test.ViewModels
{
    [TestClass]
    public class ViewModelBaseTest
    {
        [TestMethod]
        public void Constructor_Default_AssignsMessengerDefault()
        {
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();
            Assert.AreEqual(viewModelMock.Object.Messenger, Messenger.Default, "Expected Messenger.Default to be the default Messenger value.");
        }

        [TestMethod]
        public void Constructor_WithMessenger_AssignsMessengerValue()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>(new object[]{ messenger });
            Assert.AreEqual(viewModelMock.Object.Messenger, messenger, "Expected Messenger to match the provided constructor argument.");
        }

        [TestMethod]
        public void Constructor_Default_DoesNotCallOnMessengerChanged()
        {
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();
            viewModelMock.Protected().Setup("OnMessengerChanged", ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
            viewModelMock.Protected().Verify("OnMessengerChanged", Times.Never(), ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
        }

        [TestMethod]
        public void Constructor_WithMessenger_DoesNotCallOnMessengerChanged()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>(new object[] { messenger });
            viewModelMock.Protected().Setup("OnMessengerChanged", ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
            viewModelMock.Protected().Verify("OnMessengerChanged", Times.Never(), ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
        }

        [TestMethod]
        public void GetSetMessenger_WithValue_AssignsAndReturnsMessengerValue()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();

            Assert.AreNotEqual(viewModelMock.Object.Messenger, messenger, "Did not expect messenger values to match yet.");
            viewModelMock.Object.Messenger = messenger;
            Assert.AreEqual(viewModelMock.Object.Messenger, messenger, "Expected messenger values to match.");
        }

        [TestMethod]
        public void SetMessenger_WithValue_CallsOnMessengerChanged()
        {
            IMessenger messenger = new Messenger();
            Mock<ViewModelBase> viewModelMock = new Mock<ViewModelBase>();

            viewModelMock.Object.Messenger = messenger;
            viewModelMock.Protected().Setup("OnMessengerChanged", ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
            viewModelMock.Protected().Verify("OnMessengerChanged", Times.Once(), ItExpr.IsAny<IMessenger>(), ItExpr.IsAny<IMessenger>());
        }
    }
}
