using System.Collections.Generic;
using CloudSpritzers1.Src.Model.Ticket;

namespace CloudSpritzers1.Src.Service.Interfaces
{
    public interface ITicketCategoryService
    {
        TicketCategory GetCategoryById(int categoryId);

        IEnumerable<TicketCategory> GetAllCategories();
    }
}