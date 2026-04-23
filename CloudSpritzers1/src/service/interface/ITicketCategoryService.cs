using CloudSpritzers1.src.model.ticket;
using System.Collections.Generic;

namespace CloudSpritzers1.src.service.interfaces
{
    public interface ITicketCategoryService
    {
        TicketCategory GetCategoryById(int categoryId);

        IEnumerable<TicketCategory> GetAllCategories();
    }
}