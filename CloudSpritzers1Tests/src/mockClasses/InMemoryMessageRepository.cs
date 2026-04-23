using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryMessageRepository : IRepository<int, Message>
    {
        private readonly Dictionary<int, Message> _items = new Dictionary<int, Message>();
        private readonly HashSet<int> _readMessages = new HashSet<int>();
        private int _nextId = 1;

        public int CreateNewEntity(Message entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var newMessage = new Message(_nextId++, entity.GetSender(), entity.GetChat(), entity.GetMessage(), ((IMessage)entity).GetTimeStamp());
            _items[newMessage.GetId()] = newMessage;

            return newMessage.GetId();
        }

        public void DeleteById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new KeyNotFoundException($"Message with id {id} not found.");

            _items.Remove(id);
            _readMessages.Remove(id);
        }

        public IEnumerable<Message> GetAll()
        {
            return _items.Values.ToList();
        }

        public Message GetById(int id)
        {
            if (!_items.TryGetValue(id, out var entity))
                throw new KeyNotFoundException($"Message with id {id} not found.");

            return entity;
        }

        public void UpdateById(int id, Message entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!_items.ContainsKey(id)) throw new KeyNotFoundException($"Message with id {id} not found.");

            var updatedMessage = new Message(id, entity.GetSender(), entity.GetChat(), entity.GetMessage(), ((IMessage)entity).GetTimeStamp());
            _items[id] = updatedMessage;
        }

        public IEnumerable<Message> GetByChatId(int chatId)
        {
            return _items.Values
                .Where(m => m.GetChat().ChatId == chatId)
                .OrderBy(m => ((IMessage)m).GetTimeStamp())
                .ToList();
        }

        public IEnumerable<Message> GetMessagesSince(int chatId, int firstMessageId)
        {
            return _items.Values
                .Where(m => m.GetChat().ChatId == chatId && m.GetId() >= firstMessageId)
                .OrderBy(m => ((IMessage)m).GetTimeStamp())
                .ToList();
        }

        public void MarkAsRead(int messageId)
        {
            if (!_items.ContainsKey(messageId))
                throw new KeyNotFoundException($"Message with id {messageId} not found.");

            _readMessages.Add(messageId);
        }

        public bool IsRead(int messageId)
        {
            return _readMessages.Contains(messageId);
        }
    }
}