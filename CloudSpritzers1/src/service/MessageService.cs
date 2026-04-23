using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.src.model.chat;
using CloudSpritzers1.src.model.faq.bot;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.database;
using CloudSpritzers1.src.service.bot;



// FIXME: Bot Engine should not be linked to Message Service, it should be linked to a Chat...
namespace CloudSpritzers1.src.service
{
    public class MessageService
    {
        private readonly ChatDatabaseRepository _chatRepository;
        private readonly MessageDatabaseRepository _messageRepository;
        private readonly BotEngine _botEngine;

        public MessageService(
            ChatDatabaseRepository chatRepository,
            MessageDatabaseRepository messageRepository,
            BotEngine botEngine)
        {
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _botEngine = botEngine ?? throw new ArgumentNullException(nameof(botEngine));
        }

        // -------------------------------------------------------------------------
        // Send
        // -------------------------------------------------------------------------

        /// Persists the user's selected FAQ option as a message, feeds it to the BotEngine,
        /// persists the bot reply, and returns the BotMessage so the ViewModel can display it.

        public BotMessage SendMessage(int chatId, ISender sender, FAQOption selectedOption)
        {
            if (selectedOption == null)
                throw new ArgumentNullException(nameof(selectedOption));

            if(selectedOption.NextOptionId == 1)
            {
                _botEngine.ResetBotConversationStateToInitialRootNode();
            }

            Chat chat = GetActiveChat(chatId);

            // 1. Persist the user's option selection using its label as the message text.
            var userMessage = new Message(sender, chat, selectedOption.Label);
            _messageRepository.CreateNewEntity(userMessage);

            // 2. Let the bot produce a response.
            //    The strategy matches selectedOption.Label against the current node's options.
            BotMessage botReply = _botEngine.GenerateAppropriateResponseBasedOnCurrentStrategy(userMessage);

            //if(botReply.GetNextOptions().ToArray().Length == 0)
            //{
            //    _botEngine.ResetToRoot();
            //}

            // 3. Persist the bot reply.
            //    BotEngine implements ISender with id = BOT_CANNONIZED_ID (0).
            var botRow = new Message(_botEngine, chat, botReply.GetMessage());
            _messageRepository.CreateNewEntity(botRow);

            return botReply;
        }

        // -------------------------------------------------------------------------
        // Read
        // -------------------------------------------------------------------------

        /// Returns a single message, validated to belong to the given chat.
        public IMessage GetMessage(int chatId, int messageId)
        {
            IMessage message = _messageRepository.GetById(messageId);
            if (message.GetChat().ChatId != chatId)
                throw new InvalidOperationException(
                    $"Message {messageId} does not belong to chat {chatId}.");
            return message;
        }

        /// Returns all messages for a chat, ordered by timestamp ascending.
        public IEnumerable<Message> GetAllMessages(int chatId)
        {
            _ = _chatRepository.GetById(chatId);
            return _messageRepository.GetByChatId(chatId);
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private Chat GetActiveChat(int chatId)
        {
            Chat chat = _chatRepository.GetById(chatId);
            if (chat.Status != ChatStatus.Active)
                throw new InvalidOperationException($"Chat {chatId} is not active.");
            return chat;
        }
    }
}