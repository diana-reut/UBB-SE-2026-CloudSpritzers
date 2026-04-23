using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1Tests.Src.MockClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass]
    public class MessageRepositoryTests
    {
        private InMemoryMessageRepository _repository = null!;
        private ISender _testSender = null!;
        private Chat _testChat = null!;

        private class TestSender : ISender
        {
            public int RetrieveUniqueDatabaseIdentifierForBot() => 1;
            public string RetrieveConfiguredDisplayFullNameForBot() => "Test Sender";
            public string RetrieveConfiguredEmailAddressForBotContact() => "test@test.com";
        }

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryMessageRepository();
            _testSender = new TestSender();
            _testChat = new Chat(1, 101, ChatStatus.Active);
        }

        [TestMethod]
        public void Add_ValidMessage_ReturnsCorrectId()
        {
            var message = new Message(_testSender, _testChat, "Hello");
            int id = _repository.CreateNewEntity(message);
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void Add_NullMessage_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _repository.CreateNewEntity(null!));
        }

        [TestMethod]
        public void GetById_ExistingMessage_ReturnsCorrectText()
        {
            var message = new Message(_testSender, _testChat, "Hello");
            _repository.CreateNewEntity(message);

            var result = _repository.GetById(1);

            Assert.AreEqual("Hello", result.GetMessage());
        }

        [TestMethod]
        public void GetById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.GetById(999));
        }

        [TestMethod]
        public void DeleteById_ExistingId_ReducesCollectionCountToZero()
        {
            var message = new Message(_testSender, _testChat, "Hello");
            _repository.CreateNewEntity(message);

            _repository.DeleteById(1);

            Assert.AreEqual(0, _repository.GetAll().Count());
        }

        [TestMethod]
        public void DeleteById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.DeleteById(999));
        }

        [TestMethod]
        public void UpdateById_ExistingId_UpdatesTextCorrectly()
        {
            var oldMessage = new Message(_testSender, _testChat, "Old Text");
            _repository.CreateNewEntity(oldMessage);
            var updatedMessage = new Message(1, _testSender, _testChat, "New Text", DateTimeOffset.UtcNow);

            _repository.UpdateById(1, updatedMessage);
            var result = _repository.GetById(1);

            Assert.AreEqual("New Text", result.GetMessage());
        }

        [TestMethod]
        public void UpdateById_NullMessage_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _repository.UpdateById(1, null!));
        }

        [TestMethod]
        public void UpdateById_NonExistingId_ThrowsKeyNotFoundException()
        {
            var updatedMessage = new Message(999, _testSender, _testChat, "New Text", DateTimeOffset.UtcNow);
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.UpdateById(999, updatedMessage));
        }

        [TestMethod]
        public void GetByChatId_ExistingMessages_ReturnsCorrectCount()
        {
            var message1 = new Message(_testSender, _testChat, "Hello 1");
            var message2 = new Message(_testSender, _testChat, "Hello 2");
            var differentChat = new Chat(2, 101, ChatStatus.Active);
            var message3 = new Message(_testSender, differentChat, "Hello 3");
            _repository.CreateNewEntity(message1);
            _repository.CreateNewEntity(message2);
            _repository.CreateNewEntity(message3);

            var result = _repository.GetByChatId(1).ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetByChatId_ExistingMessages_ReturnsExpectedFirstMessageText()
        {
            var message1 = new Message(_testSender, _testChat, "Hello 1");
            var message2 = new Message(_testSender, _testChat, "Hello 2");
            _repository.CreateNewEntity(message1);
            _repository.CreateNewEntity(message2);

            var result = _repository.GetByChatId(1).ToList();

            Assert.AreEqual("Hello 1", result[0].GetMessage());
        }

        [TestMethod]
        public void GetMessagesSince_ExistingMessages_ReturnsCorrectCount()
        {
            var message1 = new Message(_testSender, _testChat, "Hello 1");
            var message2 = new Message(_testSender, _testChat, "Hello 2");
            var message3 = new Message(_testSender, _testChat, "Hello 3");
            _repository.CreateNewEntity(message1);
            _repository.CreateNewEntity(message2);
            _repository.CreateNewEntity(message3);

            var result = _repository.GetMessagesSince(1, 2).ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetMessagesSince_ExistingMessages_ReturnsExpectedStartingMessageText()
        {
            var message1 = new Message(_testSender, _testChat, "Hello 1");
            var message2 = new Message(_testSender, _testChat, "Hello 2");
            var message3 = new Message(_testSender, _testChat, "Hello 3");
            _repository.CreateNewEntity(message1);
            _repository.CreateNewEntity(message2);
            _repository.CreateNewEntity(message3);

            var result = _repository.GetMessagesSince(1, 2).ToList();

            Assert.AreEqual("Hello 2", result[0].GetMessage());
        }

        [TestMethod]
        public void MarkAsRead_ExistingMessage_UpdatesReadStatus()
        {
            var message = new Message(_testSender, _testChat, "Hello");
            _repository.CreateNewEntity(message);

            _repository.MarkAsRead(1);

            Assert.IsTrue(_repository.IsRead(1));
        }

        [TestMethod]
        public void MarkAsRead_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.MarkAsRead(999));
        }
    }
}