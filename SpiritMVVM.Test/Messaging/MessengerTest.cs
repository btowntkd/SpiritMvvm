using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Messaging;

namespace SpiritMVVM.Test.Messaging
{
    [TestClass]
    public class MessengerTest
    {
        public class BasicMessage : Message
        { }
        public class DerivedMessage : BasicMessage
        { }
        public class UnrelatedMessage : Message
        { }

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
                messenger.Subscribe<Message>(null, new Action<Message>(x => { /* Do nothing */ }));
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

        [TestMethod]
        public void Subscribe_NullAction_ThrowsException()
        {
            try
            {
                object token = new object();
                Messenger messenger = new Messenger();
                messenger.Subscribe<Message>(token, null);
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

        [TestMethod]
        public void Send_WithMatchingMessageType_ExecutesAction()
        {
            bool actionExecuted = false;
            object token = new object();
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(token, (msg) =>
            {
                actionExecuted = true;
            });
            messenger.Send(new BasicMessage());

            Assert.IsTrue(actionExecuted, "Subscriber did not receive message.");
        }

        [TestMethod]
        public void Send_WithUnmatchingMessageType_DoesNotExecuteAction()
        {
            bool actionExecuted = false;
            object token = new object();
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(token, (msg) =>
            {
                actionExecuted = true;
            });
            messenger.Send(new UnrelatedMessage());

            Assert.IsFalse(actionExecuted, "Subscriber received message which was not expected.");
        }

        [TestMethod]
        public void Send_WithDerivedMessage_WhenSubscriberIgnoresDerivedMessages_DoesNotExecuteAction()
        {
            bool actionExecuted = false;
            object token = new object();
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(token, false, (msg) =>
            {
                actionExecuted = true;
            });
            messenger.Send(new DerivedMessage());

            Assert.IsFalse(actionExecuted, "Subscriber received message which was not expected.");
        }

        [TestMethod]
        public void Send_WithDerivedMessage_WhenSubscriberAcceptsDerivedMessages_ExecutesAction()
        {
            bool actionExecuted = false;
            object token = new object();
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(token, true, (msg) =>
            {
                actionExecuted = true;
            });
            messenger.Send(new DerivedMessage());

            Assert.IsTrue(actionExecuted, "Subscriber did not receive message.");
        }


    }
}
