using CloudSpritzers1.src.model.chat;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1.src.service
{
    internal class MessageService
    {
        private readonly ChatDBRepository    _chatRepository;
        private readonly MessageDBRepository _messageRepository;

        public MessageService(ChatDBRepository chatRepository, MessageDBRepository messageRepository)
        {
            _chatRepository    = chatRepository    ?? throw new ArgumentNullException(nameof(chatRepository));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
        }

        // Sender is an ISender, so we extract the name and look up the chat from the DB
        public void SendMessage(int chatId, ISender sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Message text cannot be empty.", nameof(text));

            Chat chat = GetActiveChat(chatId);

            // Persist to DB — sender_id resolved via the chat's user
            _messageRepository.Add(chatId, chat.UserId, text);

            var message = new Message(sender, chat, text, isRead: false);
            chat.AddMessage(message);
        }

        // Overload for bot / system IMessage responses
        public void SendMessage(int chatId, IMessage message)
        {
            ArgumentNullException.ThrowIfNull(message);

            Chat chat = GetActiveChat(chatId);

            // Bot messages stored with sender_id = 0 (system convention)
            _messageRepository.Add(chatId, senderId: 0, message.GetMessage());

            chat.AddMessage(message);
        }

        public IEnumerable<IMessage> HandleIncomingMessage(int chatId, IMessage incomingMessage)
        {
            ArgumentNullException.ThrowIfNull(incomingMessage);

            Chat chat = GetActiveChat(chatId);

            int messageId = incomingMessage.GetId();
            if (messageId > 0)
                _messageRepository.MarkAsRead(messageId);

            chat.AddMessage(incomingMessage);

            return incomingMessage.GetNextOptions()
                .Select(opt => (IMessage)incomingMessage)   // return next bot prompts to caller
                .ToList();
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

        // Loads the chat from the DB and guards against writing to a closed session
        private Chat GetActiveChat(int chatId)
        {
            Chat chat = _chatRepository.GetById(chatId);
            if (chat.Status != ChatStatus.Active)
                throw new InvalidOperationException($"Chat {chatId} is not active.");
            return chat;
        }
    }
}
