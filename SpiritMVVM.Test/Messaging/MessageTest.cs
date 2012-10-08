using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Messaging;

namespace SpiritMVVM.Test.Messaging
{
    /// <summary>
    /// Summary description for MessageTest
    /// </summary>
    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void Constructor_Default_Success()
        {
            Message msg = new Message();
        }

        [TestMethod]
        public void Constructor_Default_NullSender()
        {
            Message msg = new Message();
            Assert.IsNull(msg.Sender, "Expected Sender to be null in default constructor.");
        }

        [TestMethod]
        public void Constructor_SenderSpecified_AssignsSender()
        {
            object sender = new object();
            Message msg = new Message(sender);

            Assert.AreEqual(sender, msg.Sender);
        }
    }
}
