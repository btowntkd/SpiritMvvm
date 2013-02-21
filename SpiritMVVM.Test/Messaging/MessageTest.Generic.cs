using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Messaging;

namespace SpiritMVVM.Test.Messaging
{
    /// <summary>
    /// Summary description for MessageTest
    /// </summary>
    [TestClass]
    public class MessageTestGeneric
    {
        /// <summary>
        /// Ensure that the Sender property matches the value provided to the constructor.
        /// </summary>
        [TestMethod]
        public void Constructor_ContentSpecified_AssignsContent()
        {
            int randomNumber = new Random().Next();
            Message<int> msg = new Message<int>(randomNumber);

            Assert.AreEqual(randomNumber, msg.Content);
        }

        /// <summary>
        /// Ensure that the Sender property matches the value provided to the constructor.
        /// </summary>
        [TestMethod]
        public void Constructor_ContentAndSenderSpecified_AssignsContentAndSender()
        {
            int randomNumber = new Random().Next();
            object sender = new object();
            Message<int> msg = new Message<int>(sender, randomNumber);

            Assert.AreEqual(randomNumber, msg.Content);
            Assert.AreEqual(sender, msg.Sender);
        }
    }
}
