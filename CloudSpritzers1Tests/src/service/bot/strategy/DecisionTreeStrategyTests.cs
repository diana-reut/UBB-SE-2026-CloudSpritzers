using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service.Bot;
using CloudSpritzers1.Src.Service.Bot.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CloudSpritzers1Tests.Src.Service.Bot.Strategy
{
    [TestClass]
    public class DecisionTreeStrategyTests
    {
        private IRepository<int, FAQNode> _mockRepo = null!;
        private DecisionTreeStrategy _strategy = null!;
        private Dictionary<int, FAQNode> _fakeDb = null!;
        private int _restartId;

        [TestInitialize]
        public void Setup()
        {
            _mockRepo = Substitute.For<IRepository<int, FAQNode>>();
            _fakeDb = new Dictionary<int, FAQNode>();

            var options1 = ImmutableArray.Create(new FAQOption("Go to 2", 2));
            _fakeDb[1] = new FAQNode(1, "Root Node", options1, false);

            _fakeDb[2] = new FAQNode(2, "Second Node", ImmutableArray<FAQOption>.Empty, true);

            _restartId = (int)BotStandardMessages.RESTART_CONVERSATION;
            if (!_fakeDb.ContainsKey(_restartId))
            {
                _fakeDb[_restartId] = new FAQNode(_restartId, "Restarting...", ImmutableArray<FAQOption>.Empty, true);
            }

            _mockRepo.GetById(Arg.Any<int>()).Returns(callInfo => _fakeDb[callInfo.Arg<int>()]);

            _strategy = new DecisionTreeStrategy(_mockRepo);
        }

        [TestMethod]
        public void ProcessIncomingUserMessage_ValidOption_AdvancesToNextNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.GetMessage().Returns("Go to 2");
            mockMessage.GetChat().Returns(new Chat(1, 1, ChatStatus.Active));

            var result = _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);

            Assert.AreEqual("Second Node", result.GetMessage());
        }

        [TestMethod]
        public void ProcessIncomingUserMessage_InvalidOption_ReturnsRestartNode()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.GetMessage().Returns("Some random gibberish");
            mockMessage.GetChat().Returns(new Chat(1, 1, ChatStatus.Active));

            var result = _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);

            Assert.AreEqual(_fakeDb[_restartId].questionText, result.GetMessage());
        }

        [TestMethod]
        public void ResetCurrentlyActiveConversationNode_ResetsToNode1()
        {
            var mockMessage = Substitute.For<IMessage>();
            mockMessage.GetMessage().Returns("Go to 2");
            mockMessage.GetChat().Returns(new Chat(1, 1, ChatStatus.Active));
            _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);

            _strategy.ResetCurrentlyActiveConversationNodeToInitialStartingPoint();

            var resultAfterReset = _strategy.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(null!, mockMessage);
            Assert.AreEqual("Second Node", resultAfterReset.GetMessage());
        }
    }
}