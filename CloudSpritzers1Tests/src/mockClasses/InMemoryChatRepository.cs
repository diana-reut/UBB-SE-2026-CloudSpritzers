using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryChatRepository : IRepository<int, Chat>
    {
        private readonly Dictionary<int, Chat> _items = new Dictionary<int, Chat>();
        private int _nextId = 1;

        public int CreateNewEntity(Chat entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            entity.ChatId = _nextId++;
            _items[entity.ChatId] = entity;
            return entity.ChatId;
        }

        public void DeleteById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new KeyNotFoundException($"Chat with id {id} not found.");

            _items.Remove(id);
        }

        public IEnumerable<Chat> GetAll()
        {
            return _items.Values.ToList();
        }

        public Chat GetById(int id)
        {
            if (!_items.TryGetValue(id, out var entity))
                throw new KeyNotFoundException($"Chat with id {id} not found.");

            return entity;
        }

        public void UpdateById(int id, Chat entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!_items.ContainsKey(id)) throw new KeyNotFoundException($"Chat with id {id} not found.");

            entity.ChatId = id;
            _items[id] = entity;
        }
    }
}