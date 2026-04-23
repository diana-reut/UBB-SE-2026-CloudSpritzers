using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.model.faq.bot;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.service.bot.strategy;

namespace CloudSpritzers1.src.service.bot
{
    public class BotEngine : ISender
    {
        public const int CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER = 0; // ChatBot is always identified as the first 
        private IBotStrategy _activeStrategyForFormulatingBotResponses;

        public BotEngine(IBotStrategy responseStrategy)
        {
            this._activeStrategyForFormulatingBotResponses = responseStrategy;
        }


        public BotMessage GenerateAppropriateResponseBasedOnCurrentStrategy(IMessage message)
        {
            return _activeStrategyForFormulatingBotResponses.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(this, message);
        }
        
        public string RetrieveConfiguredEmailAddressForBotContact()
        {
            return "customer-support@cloudspritzers.com";
        }

        public string RetrieveConfiguredDisplayFullNameForBot()
        {
            return "Carlos";
        }

        public int RetrieveUniqueDatabaseIdentifierForBot()
        {
            return CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER; 
        }

        public void ResetBotConversationStateToInitialRootNode()
        {
            _activeStrategyForFormulatingBotResponses.ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
        }
    }
}
