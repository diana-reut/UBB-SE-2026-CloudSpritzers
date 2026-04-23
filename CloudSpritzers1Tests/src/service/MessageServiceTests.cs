using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Service.Bot;
using CloudSpritzers1.Src.Service.Bot.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class MessageServiceTests
    {
        private IRepository<int, Chat> _mockChatRepo = null!;
        private IRepository<int, Message> _mockMessageRepo = null!;
        private IBotStrategy _mockStrategy = null!;
        private BotEngine _realBotEngine = null!;
        private MessageService _messageService = null!;

        private class TestSender : ISender
        {
            public int RetrieveUniqueDatabaseIdentifierForBot() => 1;
            public string RetrieveConfiguredDisplayFullNameForBot() => "Test User";
            public string RetrieveConfiguredEmailAddressForBotContact() => "user@test.com";
        }
        private TestSender _testSender = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockChatRepo = Substitute.For<IRepository<int, Chat>>();
            _mockMessageRepo = Substitute.For<IRepository<int, Message>>();
            _mockStrategy = Substitute.For<IBotStrategy>();

            _realBotEngine = new BotEngine(_mockStrategy);
            _messageService = new MessageService(_mockChatRepo, _mockMessageRepo, _realBotEngine);
            _testSender = new TestSender();
        }

        [TestMethod]
        public void Constructor_NullChatRepo_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new MessageService(null!, _mockMessageRepo, _realBotEngine));
        }

        [TestMethod]
        public void Constructor_NullMessageRepo_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new MessageService(_mockChatRepo, null!, _realBotEngine));
        }

        [TestMethod]
        public void Constructor_NullBotEngine_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new MessageService(_mockChatRepo, _mockMessageRepo, null!));
        }

        [TestMethod]
        public void SendMessage_NullOption_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _messageService.SendMessage(1, _testSender, null!));
        }

        [TestMethod]
        public void SendMessage_InactiveChat_ThrowsInvalidOperationException()
        {
            var closedChat = new Chat(1, 101, ChatStatus.Closed);
            _mockChatRepo.GetById(1).Returns(closedChat);
            var option = new FAQOption("Hello", 2);

            Assert.ThrowsExactly<InvalidOperationException>(() => _messageService.SendMessage(1, _testSender, option));
        }

        [TestMethod]
        public void SendMessage_InactiveChat_ThrowsCorrectMessage()
        {
            var closedChat = new Chat(1, 101, ChatStatus.Closed);
            _mockChatRepo.GetById(1).Returns(closedChat);
            var option = new FAQOption("Hello", 2);

            var ex = Assert.ThrowsExactly<InvalidOperationException>(() => _messageService.SendMessage(1, _testSender, option));
            Assert.AreEqual("Chat 1 is not active.", ex.Message);
        }

        [TestMethod]
        public void SendMessage_ValidInput_ReturnsCorrectBotReplyMessage()
        {
            var activeChat = new Chat(1, 101, ChatStatus.Active);
            _mockChatRepo.GetById(1).Returns(activeChat);
            var option = new FAQOption("Help me", 2);
            var expectedReply = new BotMessage.BotMessageBuilder(_realBotEngine, activeChat, 2).WithMessage("I can help").Build();
            _mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(Arg.Any<BotEngine>(), Arg.Any<IMessage>()).Returns(expectedReply);

            var result = _messageService.SendMessage(1, _testSender, option);

            Assert.AreEqual("I can help", result.GetMessage());
        }

        [TestMethod]
        public void SendMessage_ValidInput_PersistsBothUserAndBotMessages()
        {
            var activeChat = new Chat(1, 101, ChatStatus.Active);
            _mockChatRepo.GetById(1).Returns(activeChat);
            var option = new FAQOption("Help me", 2);
            var expectedReply = new BotMessage.BotMessageBuilder(_realBotEngine, activeChat, 2).WithMessage("I can help").Build();
            _mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(Arg.Any<BotEngine>(), Arg.Any<IMessage>()).Returns(expectedReply);

            _messageService.SendMessage(1, _testSender, option);

            _mockMessageRepo.Received(2).CreateNewEntity(Arg.Any<Message>());
        }

        [TestMethod]
        public void SendMessage_OptionId1_ResetsBotStrategy()
        {
            var activeChat = new Chat(1, 101, ChatStatus.Active);
            _mockChatRepo.GetById(1).Returns(activeChat);
            var restartOption = new FAQOption("Restart", 1);
            var expectedReply = new BotMessage.BotMessageBuilder(_realBotEngine, activeChat, 1).WithMessage("Restarting").Build();
            _mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(Arg.Any<BotEngine>(), Arg.Any<IMessage>()).Returns(expectedReply);

            _messageService.SendMessage(1, _testSender, restartOption);

            _mockStrategy.Received(1).ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
        }

        [TestMethod]
        public void GetMessage_WrongChat_ThrowsInvalidOperationException()
        {
            var wrongChat = new Chat(99, 101, ChatStatus.Active);
            var message = new Message(_testSender, wrongChat, "Text");
            _mockMessageRepo.GetById(5).Returns(message);

            Assert.ThrowsExactly<InvalidOperationException>(() => _messageService.GetMessage(1, 5));
        }

        [TestMethod]
        public void GetMessage_WrongChat_ThrowsCorrectErrorMessage()
        {
            var wrongChat = new Chat(99, 101, ChatStatus.Active);
            var message = new Message(_testSender, wrongChat, "Text");
            _mockMessageRepo.GetById(5).Returns(message);

            var ex = Assert.ThrowsExactly<InvalidOperationException>(() => _messageService.GetMessage(1, 5));
            Assert.AreEqual("Message 5 does not belong to chat 1.", ex.Message);
        }

        [TestMethod]
        public void GetMessage_ValidMessage_ReturnsMessageText()
        {
            var correctChat = new Chat(1, 101, ChatStatus.Active);
            var message = new Message(_testSender, correctChat, "Correct Text");
            _mockMessageRepo.GetById(5).Returns(message);

            var result = _messageService.GetMessage(1, 5);

            Assert.AreEqual("Correct Text", result.GetMessage());
        }

        [TestMethod]
        public void GetAllMessages_ReturnsCorrectCount()
        {
            var chat1 = new Chat(1, 101, ChatStatus.Active);
            var msg1 = new Message(1, _testSender, chat1, "A", DateTimeOffset.UtcNow);
            var msg2 = new Message(2, _testSender, chat1, "B", DateTimeOffset.UtcNow);
            _mockMessageRepo.GetAll().Returns(new List<Message> { msg1, msg2 });

            var result = _messageService.GetAllMessages(1).ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetAllMessages_OrdersByTimestampAscending()
        {
            var chat1 = new Chat(1, 101, ChatStatus.Active);
            var earlier = new Message(1, _testSender, chat1, "Earlier", DateTimeOffset.UtcNow.AddMinutes(-10));
            var later = new Message(2, _testSender, chat1, "Later", DateTimeOffset.UtcNow);
            _mockMessageRepo.GetAll().Returns(new List<Message> { later, earlier });

            var result = _messageService.GetAllMessages(1).ToList();

            Assert.AreEqual("Earlier", result[0].GetMessage());
        }

        [TestMethod]
        public void GetAllMessages_FiltersOutOtherChats()
        {
            var chat1 = new Chat(1, 101, ChatStatus.Active);
            var chat2 = new Chat(2, 202, ChatStatus.Active);
            var msg1 = new Message(1, _testSender, chat1, "Keep", DateTimeOffset.UtcNow);
            var msg2 = new Message(2, _testSender, chat2, "Discard", DateTimeOffset.UtcNow);
            _mockMessageRepo.GetAll().Returns(new List<Message> { msg1, msg2 });

            var result = _messageService.GetAllMessages(1).ToList();

            Assert.AreEqual("Keep", result[0].GetMessage());
        }
    }
}