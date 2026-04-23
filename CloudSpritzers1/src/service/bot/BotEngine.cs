using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service.Bot.Strategy;

namespace CloudSpritzers1.Src.Service.Bot
{
    public class BotEngine : ISender
    {
        public const int CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER = 0; // ChatBot is always identified as the first
        private IBotStrategy activeStrategyForFormulatingBotResponses;

        public BotEngine(IBotStrategy responseStrategy)
        {
            this.activeStrategyForFormulatingBotResponses = responseStrategy;
        }

        public BotMessage GenerateAppropriateResponseBasedOnCurrentStrategy(IMessage message)
        {
            return activeStrategyForFormulatingBotResponses.ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(this, message);
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
            activeStrategyForFormulatingBotResponses.ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
        }
    }
}
