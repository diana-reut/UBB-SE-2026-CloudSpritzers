using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service.Bot;

namespace CloudSpritzers1.Src.Service
{
    public class MessageService
    {
        private readonly IRepository<int, Chat> chatRepository;
        private readonly IRepository<int, Message> messageRepository;
        private readonly BotEngine botEngine;

        public MessageService(
            IRepository<int, Chat> chatRepository,
            IRepository<int, Message> messageRepository,
            BotEngine botEngine)
        {
            this.chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            this.messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            this.botEngine = botEngine ?? throw new ArgumentNullException(nameof(botEngine));
        }

        public BotMessage SendMessage(int chatId, ISender sender, FAQOption selectedOption)
        {
            if (selectedOption == null)
            {
                throw new ArgumentNullException(nameof(selectedOption));
            }
            if (selectedOption.nextOptionId == 1)
            {
                botEngine.ResetBotConversationStateToInitialRootNode();
            }

            Chat chat = GetActiveChat(chatId);

            var userMessage = new Message(sender, chat, selectedOption.label);
            messageRepository.CreateNewEntity(userMessage);

            BotMessage botReply = botEngine.GenerateAppropriateResponseBasedOnCurrentStrategy(userMessage);

            var botRow = new Message(botEngine, chat, botReply.GetMessage());
            messageRepository.CreateNewEntity(botRow);

            return botReply;
        }

        public IMessage GetMessage(int chatId, int messageId)
        {
            IMessage message = messageRepository.GetById(messageId);
            if (message.GetChat().ChatId != chatId)
            {
                throw new InvalidOperationException($"Message {messageId} does not belong to chat {chatId}.");
            }
            return message;
        }

        public IEnumerable<Message> GetAllMessages(int chatId)
        {
            _ = chatRepository.GetById(chatId);

            return messageRepository.GetAll()
                .Where(m => m.GetChat().ChatId == chatId)
                .OrderBy(m => ((IMessage)m).GetTimeStamp());
        }

        private Chat GetActiveChat(int chatId)
        {
            Chat chat = chatRepository.GetById(chatId);
            if (chat.Status != ChatStatus.Active)
            {
                throw new InvalidOperationException($"Chat {chatId} is not active.");
            }
            return chat;
        }
    }
}