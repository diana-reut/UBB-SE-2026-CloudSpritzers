using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.model.faq.bot;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.database;

namespace CloudSpritzers1.src.service.bot.strategy
{
    public class DecisionTreeStrategy : IBotStrategy
    {
        const int CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER = -1;
        [AllowNull]
        private FAQNode _currentlyActiveConversationDecisionTreeNode;

        private DecisionTreeRepository _repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes;

        public DecisionTreeStrategy(DecisionTreeRepository faqRepository)
        {
            this._repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes = faqRepository;
            this._currentlyActiveConversationDecisionTreeNode = _repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(1);
        }

        public BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngine activeBotEngineInstance, IMessage incomingUserMessage)
        {
            string extractedTextContentFromIncomingUserMessage = incomingUserMessage.GetMessage();

            FAQOption? selectedUserOptionMatchingIncomingMessageText = _currentlyActiveConversationDecisionTreeNode.Options.FirstOrDefault((option) => option.Label.Equals(extractedTextContentFromIncomingUserMessage));
            if (selectedUserOptionMatchingIncomingMessageText == null)
            {
                return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER,
                    _repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById((int)BotStandardMessages.RESTART_CONVERSATION)).Build();
            }

            FAQNode nextQuestion = _repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(selectedUserOptionMatchingIncomingMessageText.NextOptionId);
            _currentlyActiveConversationDecisionTreeNode = nextQuestion;

            return new BotMessage.BotMessageBuilder(activeBotEngineInstance, incomingUserMessage.GetChat(), CONSTANT_VALUE_REPRESENTING_UNASSIGNED_DATABASE_IDENTIFIER, nextQuestion).Build();
        }

        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint()
        {
            _currentlyActiveConversationDecisionTreeNode = _repositoryForAccessingFrequentlyAskedQuestionsDecisionNodes.GetById(1);
        }
    }
}
