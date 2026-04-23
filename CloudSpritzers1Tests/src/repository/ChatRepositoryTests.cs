using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1Tests.Src.MockClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass]
    public class ChatRepositoryTests
    {
        private IRepository<int, Chat> _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryChatRepository();
        }

        [TestMethod]
        public void Add_ValidChat_ReturnsCorrectId()
        {
            var chat = new Chat(0, 101, default(ChatStatus));
            int id = _repository.CreateNewEntity(chat);
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void Add_NullChat_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _repository.CreateNewEntity(null!));
        }

        [TestMethod]
        public void GetById_ExistingChat_ReturnsCorrectUserId()
        {
            var chat = new Chat(0, 202, default(ChatStatus));
            _repository.CreateNewEntity(chat);

            var result = _repository.GetById(1);

            Assert.AreEqual(202, result.UserId);
        }

        [TestMethod]
        public void GetById_ExistingChat_ReturnsCorrectStatus()
        {
            var chat = new Chat(0, 202, ChatStatus.Closed);
            _repository.CreateNewEntity(chat);

            var result = _repository.GetById(1);

            Assert.AreEqual(ChatStatus.Closed, result.Status);
        }

        [TestMethod]
        public void GetById_ReturnsCorrectChatId()
        {
            var chat = new Chat(99, 101, default(ChatStatus));
            int assignedId = _repository.CreateNewEntity(chat);

            var result = _repository.GetById(assignedId);

            Assert.AreEqual(assignedId, result.ChatId);
        }

        [TestMethod]
        public void GetById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.GetById(999));
        }

        [TestMethod]
        public void DeleteById_ExistingId_ReducesCollectionCountToZero()
        {
            var chat = new Chat(0, 101, default(ChatStatus));
            _repository.CreateNewEntity(chat);

            _repository.DeleteById(1);

            Assert.AreEqual(0, _repository.GetAll().Count());
        }

        [TestMethod]
        public void DeleteById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.DeleteById(999));
        }

        [TestMethod]
        public void UpdateById_ExistingId_UpdatesUserId()
        {
            var oldChat = new Chat(0, 100, default(ChatStatus));
            _repository.CreateNewEntity(oldChat);
            var updatedChat = new Chat(0, 999, default(ChatStatus));

            _repository.UpdateById(1, updatedChat);
            var result = _repository.GetById(1);

            Assert.AreEqual(999, result.UserId);
        }

        [TestMethod]
        public void UpdateById_ExistingId_UpdatesStatus()
        {
            var oldChat = new Chat(0, 100, default(ChatStatus));
            _repository.CreateNewEntity(oldChat);
            var updatedChat = new Chat(0, 100, ChatStatus.Closed);

            _repository.UpdateById(1, updatedChat);
            var result = _repository.GetById(1);

            Assert.AreEqual(ChatStatus.Closed, result.Status);
        }

        [TestMethod]
        public void UpdateById_NullChat_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _repository.UpdateById(1, null!));
        }

        [TestMethod]
        public void UpdateById_NonExistingId_ThrowsKeyNotFoundException()
        {
            var updatedChat = new Chat(999, 100, default(ChatStatus));
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.UpdateById(999, updatedChat));
        }
    }
}