using CloudSpritzers1.src.model.chat;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.service.bot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1.src.service
{
    internal class MessageService
    {
        private readonly ChatDBRepository _chatRepository;
        private readonly MessageDBRepository _messageRepository;
        private readonly BotEngine _botEngine;

        public MessageService(
            ChatDBRepository chatRepository,
            MessageDBRepository messageRepository,
            BotEngine botEngine)
        {
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _botEngine = botEngine ?? throw new ArgumentNullException(nameof(botEngine));
        }

        public void SendMessage(int chatId, ISender sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Message text cannot be empty.", nameof(text));

            Chat chat = GetActiveChat(chatId);

            var message = new Message(sender, chat, text, isRead: false);
            _messageRepository.Add(message);
            chat.AddMessage(message);

            if (sender is not BotEngine)
                HandleIncomingMessage(chatId, message);
        }

        public void SendMessage(int chatId, IMessage botResponse)
        {
            ArgumentNullException.ThrowIfNull(botResponse);

            Chat chat = GetActiveChat(chatId);

            var stored = new Message(
                _botEngine,
                chat,
                botResponse.GetMessage(),
                isRead: false
            );
            _messageRepository.Add(stored);

            chat.AddMessage(botResponse);
        }

        private void HandleIncomingMessage(int chatId, IMessage incomingMessage)
        {
            int messageId = incomingMessage.GetId();
            if (messageId > 0)
                _messageRepository.MarkAsRead(messageId);

            BotMessage botReply = _botEngine.Respond(incomingMessage);

            SendMessage(chatId, botReply);
        }

        public IMessage GetMessage(int chatId, int messageId)
        {
            _ = GetActiveChat(chatId);
            return _messageRepository.GetById(messageId);
        }

        public IEnumerable<IMessage> GetAllMessages(int chatId)
        {
            _ = GetActiveChat(chatId);
            return _messageRepository.GetByChatId(chatId);
        }

        public IEnumerable<IMessage> GetMessagesSince(int chatId, int firstMessageId)
        {
            _ = GetActiveChat(chatId);
            return _messageRepository.GetMessagesSince(chatId, firstMessageId);
        }

        private Chat GetActiveChat(int chatId)
        {
            Chat chat = _chatRepository.GetById(chatId);
            if (chat.Status != ChatStatus.Active)
                throw new InvalidOperationException($"Chat {chatId} is not active.");
            return chat;
        }
    }
}