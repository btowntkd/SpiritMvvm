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
        /// <summary>
        /// Ensure that the default constructor is successful in creating a new instance.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_Success()
        {
            Message msg = new Message();
        }

        /// <summary>
        /// Ensure that the default constructor sets the Sender property to null.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_NullSender()
        {
            Message msg = new Message();
            Assert.IsNull(msg.Sender, "Expected Sender to be null in default constructor.");
        }

        /// <summary>
        /// Ensure that the Sender property matches the value provided to the constructor.
        /// </summary>
        [TestMethod]
        public void Constructor_SenderSpecified_AssignsSender()
        {
            object sender = new object();
            Message msg = new Message(sender);

            Assert.AreEqual(sender, msg.Sender);
        }
    }
}
