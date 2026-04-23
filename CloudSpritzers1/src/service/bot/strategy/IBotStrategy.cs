using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.model.message;

namespace CloudSpritzers1.src.service.bot.strategy
{
    public interface IBotStrategy
    {
        BotMessage ProcessIncomingUserMessageAndDetermineNextDecisionTreeNode(BotEngine activeBotEngineInstance, IMessage incomingUserMessage);

        public void ResetCurrentlyActiveConversationNodeToInitialStartingPoint();
    }
}
