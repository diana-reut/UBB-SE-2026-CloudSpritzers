using CloudSpritzers1.src.model.message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1.src.service
{
    internal class MessageService
    {
        private readonly Dictionary<int, List<IMessage>> _chatMessages = new();

        public void SendMessage(int chatId, IResponder sender, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Message text cannot be empty.", nameof(text));

            var message = new Message(sender, chatId, text, isRead: false);
            GetOrCreateList(chatId).Add(message);
        }

        public void SendMessage(int chatId, IMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            GetOrCreateList(chatId).Add(message);
        }

        public IEnumerable<IMessage> HandleIncomingMessage(int chatId, IMessage incomingMessage)
        {
            if (incomingMessage == null)
                throw new ArgumentNullException(nameof(incomingMessage));

            GetOrCreateList(chatId).Add(incomingMessage);
            return incomingMessage.GetNextOptions();
        }

        public IMessage GetMessage(int chatId, int messageId)
        {
            IMessage message = GetOrCreateList(chatId)
                .FirstOrDefault(m => m.GetId() == messageId);

            if (message == null)
                throw new KeyNotFoundException($"Message {messageId} not found in chat {chatId}.");

            return message;
        }

        public IEnumerable<IMessage> GetAllMessages(int chatId)
        {
            return GetOrCreateList(chatId).AsReadOnly();
        }

        public IEnumerable<IMessage> GetMessagesSince(int chatId, int firstMessageId)
        {
            return GetOrCreateList(chatId)
                .Where(m => m.GetId() >= firstMessageId)
                .ToList();
        }

        private List<IMessage> GetOrCreateList(int chatId)
        {
            if (!_chatMessages.ContainsKey(chatId))
                _chatMessages[chatId] = new List<IMessage>();

            return _chatMessages[chatId];
        }
    }
}