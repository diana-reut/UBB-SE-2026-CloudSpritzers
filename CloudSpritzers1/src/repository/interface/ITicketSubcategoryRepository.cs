using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Ticket;

namespace CloudSpritzers1.Src.Repository
{
    public interface ITicketSubcategoryRepository
    {
        IEnumerable<TicketSubcategory> GetAll();

        TicketSubcategory GetById(int subcategoryId);

        IEnumerable<TicketSubcategory> GetByCategoryId(int categoryId);
    }
}