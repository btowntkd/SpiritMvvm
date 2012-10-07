using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Messaging;

namespace SpiritMVVM.Test.Messaging
{
    [TestClass]
    public class MessengerTest
    {
        [TestMethod]
        public void Constructor_Default_Success()
        {
            Messenger messenger = new Messenger();
        }

        [TestMethod]
        public void Subscribe_NullToken_ThrowsException()
        {          
            try
            {
                Messenger messenger = new Messenger();
                messenger.Subscribe<int>(null, new Action<int>(x => { /* Do nothing */ }));
            }
            catch (ArgumentNullException)
            {
                /* PASS */
            }
            catch (Exception)
            {
                Assert.Fail("Exception was not of the expected type.");
            }
        }
    }
}
