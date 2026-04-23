using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.model.ticket;

namespace CloudSpritzers1.src.repository
{
    public interface ITicketSubcategoryRepository 
    {
        IEnumerable<TicketSubcategory> GetAll();

        TicketSubcategory GetById(int subcategoryId);

        IEnumerable<TicketSubcategory> GetByCategoryId(int categoryId);
    }
}