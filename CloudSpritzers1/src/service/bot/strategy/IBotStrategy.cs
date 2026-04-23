using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Message;

namespace CloudSpritzers1.Src.Service.Bot.Strategy
{
    public interface IBotStrategy
    {
        BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngine activeBotEngineInstance, IMessage incomingUserMessage);

        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
    }
}
