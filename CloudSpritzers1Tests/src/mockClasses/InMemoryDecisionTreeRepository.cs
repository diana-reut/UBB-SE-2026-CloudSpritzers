using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryDecisionTreeRepository : IRepository<int, FAQNode>
    {
        private readonly Dictionary<int, FAQNode> _items = new Dictionary<int, FAQNode>();
        private int _nextId = 1;

        public int CreateNewEntity(FAQNode entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var newEntity = entity with { faqNodeId = _nextId++ };
            _items[newEntity.faqNodeId] = newEntity;
            return newEntity.faqNodeId;
        }

        public void DeleteById(int id)
        {
            if (!_items.ContainsKey(id))
                throw new KeyNotFoundException($"FAQNode with id {id} not found.");

            _items.Remove(id);
        }

        public IEnumerable<FAQNode> GetAll()
        {
            return _items.Values.ToList();
        }

        public FAQNode GetById(int id)
        {
            if (!_items.TryGetValue(id, out var entity))
                throw new KeyNotFoundException($"FAQNode with id {id} not found.");

            return entity;
        }

        public void UpdateById(int id, FAQNode entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!_items.ContainsKey(id)) throw new KeyNotFoundException($"FAQNode with id {id} not found.");

            var updatedEntity = entity with { faqNodeId = id };
            _items[id] = updatedEntity;
        }
    }
}