using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Service.Bot;
using CloudSpritzers1.Src.Service.Bot.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CloudSpritzers1Tests.Src.Service.Bot
{
    [TestClass]
    public class BotEngineTests
    {
        private IBotStrategy _mockStrategy = null!;
        private BotEngine _botEngine = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockStrategy = Substitute.For<IBotStrategy>();

            _botEngine = new BotEngine(_mockStrategy);
        }

        [TestMethod]
        public void GenerateAppropriateResponse_ValidMessage_ReturnsStrategyResult()
        {
            var mockIncomingMessage = Substitute.For<IMessage>();
            var dummyChat = new Chat(1, 1, ChatStatus.Active);

            var expectedResponse = new BotMessage.BotMessageBuilder(_botEngine, dummyChat, 1)
                .WithMessage("I am the mocked strategy response")
                .Build();

            _mockStrategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(_botEngine, mockIncomingMessage)
                .Returns(expectedResponse);

            var result = _botEngine.GenerateAppropriateResponseBasedOnCurrentStrategy(mockIncomingMessage);

            Assert.AreEqual(expectedResponse, result);
        }

        [TestMethod]
        public void ResetBotConversationState_CallsStrategyResetMethodExactlyOnce()
        {
            _botEngine.ResetBotConversationStateToInitialRootNode();

            _mockStrategy.Received(1).ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
        }

        [TestMethod]
        public void RetrieveConfiguredEmailAddressForBotContact_ReturnsCorrectEmail()
        {
            var result = _botEngine.RetrieveConfiguredEmailAddressForBotContact();

            Assert.AreEqual("customer-support@cloudspritzers.com", result);
        }

        [TestMethod]
        public void RetrieveConfiguredDisplayFullNameForBot_ReturnsCarlos()
        {
            var result = _botEngine.RetrieveConfiguredDisplayFullNameForBot();

            Assert.AreEqual("Carlos", result);
        }

        [TestMethod]
        public void RetrieveUniqueDatabaseIdentifierForBot_ReturnsZero()
        {
            var result = _botEngine.RetrieveUniqueDatabaseIdentifierForBot();

            Assert.AreEqual(0, result);
        }
    }
}