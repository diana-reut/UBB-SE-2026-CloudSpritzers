using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryTicketSubcategoryRepository : ITicketSubcategoryRepository
    {
        private List<TicketSubcategory> _subcategories = new List<TicketSubcategory>();

        
        public int CreateNewEntity(TicketSubcategory elem)
        {
            if (elem == null) throw new ArgumentNullException(nameof(elem));

            _subcategories.Add(elem);
            return elem.SubcategoryId;
        }

        public TicketSubcategory GetById(int id)
        {
            var sub = _subcategories.FirstOrDefault(s => s.SubcategoryId == id);

            if (sub == null)
            {
                
                throw new KeyNotFoundException($"Subcategory with id {id} not found.");
            }

            return sub;
        }

        public IEnumerable<TicketSubcategory> GetAll()
        {
            return _subcategories;
        }

        public IEnumerable<TicketSubcategory> GetByCategoryId(int categoryId)
        {
            
            return _subcategories.Where(s => s.ParentCategory.CategoryId == categoryId);
        }
    }
}
