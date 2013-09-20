using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpiritMVVM.Messaging;
using System;
using System.Threading;

namespace SpiritMVVM.Test.Messaging
{
    /// <summary>
    /// Unit tests for the <see cref="Messenger"/> class.
    /// </summary>
    [TestClass]
    public class MessengerTest
    {
        internal class BasicMessage : Message
        { }

        internal class DerivedMessage : BasicMessage
        { }

        internal class UnrelatedMessage : Message
        { }

        internal class LargeToken
        {
            private readonly byte[] memoryGuzzler = new byte[100000];
        }

        private static readonly object TestLock = new object();

        /// <summary>
        /// Sets up each unit test and ensures that only one will
        /// execute at a time.
        /// </summary>
        [TestInitialize]
        public void TestStart()
        {
            Monitor.Enter(TestLock);
        }

        /// <summary>
        /// Tears down a unit test, releasing the exclusive lock
        /// and allowing the next unit test to execute.
        /// </summary>
        [TestCleanup]
        public void TestEnd()
        {
            Monitor.Exit(TestLock);
        }

        /// <summary>
        /// Ensures a user can create a new default instance of the Messenger class.
        /// </summary>
        [TestMethod]
        public void Constructor_Default_Success()
        {
            Messenger messenger = new Messenger();
        }

        /// <summary>
        /// Ensures that Subscribe will not accept a null "Token" argument.
        /// </summary>
        [TestMethod]
        public void Subscribe_NullToken_ThrowsArgumentNullException()
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

        /// <summary>
        /// Ensures that Subscribe will not accept a null "Action" argument.
        /// </summary>
        [TestMethod]
        public void Subscribe_NullAction_ThrowsArgumentNullException()
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

        /// <summary>
        /// Ensures that Subscribe will accept valid method arguments.
        /// </summary>
        [TestMethod]
        public void Subscribe_ValidArgs_Success()
        {
            object token = new object();
            Messenger messenger = new Messenger();
            messenger.Subscribe<Message>(token, new Action<Message>((msg) => { /* Do nothing */ }));
        }

        /// <summary>
        /// Ensures that calling "Send" with a message type which the subscriber has subscribed
        /// to, will cause the subscriber to receive that message.
        /// </summary>
        [TestMethod]
        public void Send_WithMatchingMessageType_ExecutesAction()
        {
            bool actionExecuted = false;
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(this, (msg) =>
            {
                actionExecuted = true;
            });
            messenger.Send(new BasicMessage());

            Assert.IsTrue(actionExecuted, "Subscriber did not receive message.");
        }

        /// <summary>
        /// Ensures that the message delivered by the "Send" method is the exact message which
        /// was provided to the "Send" method as an argument.
        /// </summary>
        [TestMethod]
        public void Send_WithMatchingMessageType_ReceivesSameMessageReference()
        {
            BasicMessage sentMessage = new BasicMessage();
            BasicMessage receivedMessage = null;
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(this, (msg) =>
            {
                receivedMessage = msg;
            });
            messenger.Send(sentMessage);

            Assert.AreEqual(sentMessage, receivedMessage, "Expected sent message and received messages to be identical references.");
        }

        /// <summary>
        /// Ensures that calling "Send" with a message type which is unrelated to a subscriber's
        /// desired message type will not cause the subscriber to receive the message.
        /// </summary>
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

        /// <summary>
        /// Ensures that calling "Send" with a derived message will cause a subscriber to
        /// NOT receive the message, if the subscriber opted to NOT receive derived message types.
        /// </summary>
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

        /// <summary>
        /// Ensures that calling "Send" with a derived message will cause a subscriber to
        /// receive the message, if the subscriber opted to receive derived message types.
        /// </summary>
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

        /// <summary>
        /// Ensures that Unsubscribe dependably removes subscribers from the list.
        /// </summary>
        [TestMethod]
        public void Unsubscribe_ThenSendMatchingMessageType_DoesNotExecuteAction()
        {
            bool actionExecuted = false;
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(this, (msg) =>
            {
                actionExecuted = true;
            });

            messenger.Unsubscribe<BasicMessage>(this);
            messenger.Send(new BasicMessage());

            Assert.IsFalse(actionExecuted, "Subscriber should not receive message after Unsibscribing.");
        }

        /// <summary>
        /// Ensures that the SendAsync method returns immediately, and executes the Send action
        /// as a background Task.
        /// </summary>
        [TestMethod]
        public void SendAsync_WithMatchingMessageType_ReturnsImmediately_ExecutesActionLater()
        {
            AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
            bool actionExecuted = false;
            Messenger messenger = new Messenger();
            messenger.Subscribe<BasicMessage>(this, (msg) =>
            {
                actionExecuted = true;
                messageReceivedEvent.Set();
            });
            messenger.SendAsync(new BasicMessage());
            Assert.IsFalse(actionExecuted, "Did not expect message yet.");

            messageReceivedEvent.WaitOne(100);
            Assert.IsTrue(actionExecuted, "Subscriber did not receive message within reasonable wait time.");
        }

        /// <summary>
        /// Ensures that the Subscribe method only stores <see cref="WeakReference"/> to the
        /// recipient token, so that recipients can be GarbageCollected and not leak memory, due
        /// to references existing in the messenger.
        /// </summary>
        [TestMethod]
        public void Subscribe_DeadRecipient_DoesNotLeakMemory()
        {
            Messenger messenger = new Messenger();
            int x = 0;
            object token = null;
            Action<Message> messageAction = new Action<Message>((msg) => { /* Do nothing */ });
            Message message = new Message();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long beforeMemory = GC.GetTotalMemory(false);

            for (x = 0; x < 10000; x++)
            {
                token = new LargeToken();
                messenger.Subscribe(token, messageAction);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            messenger.Unsubscribe<Message>(token);
            token = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long afterMemory = GC.GetTotalMemory(false);

            long memDifference = afterMemory - beforeMemory;
            Assert.IsTrue(memDifference < 10000, "Possible memory leak in Messenger");
        }

        /// <summary>
        /// Ensures that the default, static instance is not null.
        /// </summary>
        [TestMethod]
        public void Default_Instance_IsNotNull()
        {
            Assert.IsNotNull(Messenger.Default, "Default instance should not be null");
        }
    }
}