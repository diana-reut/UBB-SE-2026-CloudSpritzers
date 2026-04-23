using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryTicketCategoryRepository : ITicketCategoryRepository
    {
        private List<TicketCategory> _categories = new List<TicketCategory>();

        public int CreateNewEntity(TicketCategory elem)
        {
            if (elem == null) throw new ArgumentNullException(nameof(elem));

            
            if (_categories.Any(c => c.CategoryId == elem.CategoryId))
                throw new ArgumentException("This TicketCategory already exists.");

            _categories.Add(elem);
            return elem.CategoryId;
        }

        public TicketCategory GetById(int id)
        {
            var category = _categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                throw new KeyNotFoundException($"Category with id {id} was not found.");

            return category;
        }

        public IEnumerable<TicketCategory> GetAll()
        {
            return _categories;
        }
    }
}