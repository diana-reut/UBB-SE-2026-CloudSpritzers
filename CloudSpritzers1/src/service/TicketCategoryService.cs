using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TicketCategoryService
{
    private readonly TicketCategoryRepository _categoryRepo;

    public TicketCategoryService(TicketCategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public TicketCategory GetCategoryById(int categoryId)
    {
        return _categoryRepo.GetById(categoryId); 
    }
    public IEnumerable<TicketCategory> GetAllCategories()
    {
        return _categoryRepo.GetAll();
    }
}