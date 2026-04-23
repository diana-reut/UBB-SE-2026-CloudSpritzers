using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class ChatServiceTests
    {
        private IRepository<int, Chat> _mockChatRepo = null!;
        private ChatService _chatService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockChatRepo = Substitute.For<IRepository<int, Chat>>();
            _chatService = new ChatService(_mockChatRepo);
        }

        [TestMethod]
        public void OpenChat_ValidUserId_ReturnsChatWithCorrectId()
        {
            _mockChatRepo.CreateNewEntity(Arg.Any<Chat>()).Returns(5);

            var result = _chatService.OpenChat(101);

            Assert.AreEqual(5, result.ChatId);
        }

        [TestMethod]
        public void OpenChat_ValidUserId_ReturnsChatWithCorrectUserId()
        {
            _mockChatRepo.CreateNewEntity(Arg.Any<Chat>()).Returns(5);

            var result = _chatService.OpenChat(101);

            Assert.AreEqual(101, result.UserId);
        }

        [TestMethod]
        public void OpenChat_ValidUserId_ReturnsChatWithActiveStatus()
        {
            _mockChatRepo.CreateNewEntity(Arg.Any<Chat>()).Returns(5);

            var result = _chatService.OpenChat(101);

            Assert.AreEqual(ChatStatus.Active, result.Status);
        }

        [TestMethod]
        public void OpenChat_RepositoryThrowsException_ThrowsException()
        {
            _mockChatRepo.CreateNewEntity(Arg.Any<Chat>()).Returns(x => throw new Exception("Database error"));

            Assert.ThrowsExactly<Exception>(() => _chatService.OpenChat(101));
        }

        [TestMethod]
        public void OpenChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            _mockChatRepo.CreateNewEntity(Arg.Any<Chat>()).Returns(x => throw new Exception("Database error"));

            var ex = Assert.ThrowsExactly<Exception>(() => _chatService.OpenChat(101));

            Assert.AreEqual("Database error", ex.Message);
        }

        [TestMethod]
        public void CloseChat_ExistingChat_CallsRepositoryUpdateWithClosedStatus()
        {
            var fakeChat = new Chat(1, 101, ChatStatus.Active);
            _mockChatRepo.GetById(1).Returns(fakeChat);

            _chatService.CloseChat(1);

            _mockChatRepo.Received(1).UpdateById(1, Arg.Is<Chat>(c => c.Status == ChatStatus.Closed));
        }

        [TestMethod]
        public void CloseChat_RepositoryThrowsException_ThrowsException()
        {
            _mockChatRepo.GetById(999).Returns(x => throw new KeyNotFoundException("Chat not found"));

            Assert.ThrowsExactly<Exception>(() => _chatService.CloseChat(999));
        }

        [TestMethod]
        public void CloseChat_RepositoryThrowsException_ThrowsCorrectErrorMessage()
        {
            _mockChatRepo.GetById(999).Returns(x => throw new KeyNotFoundException("Chat not found"));

            var ex = Assert.ThrowsExactly<Exception>(() => _chatService.CloseChat(999));

            Assert.AreEqual("Chat not found", ex.Message);
        }
    }
}