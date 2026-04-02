using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TicketSubcategoryService
{
    private readonly TicketSubcategoryRepository _subcategoryRepo;

    public TicketSubcategoryService(TicketSubcategoryRepository subcategoryRepo)
    {
        _subcategoryRepo = subcategoryRepo;
    }

    // Fetch all subcategories for a category
    public IEnumerable<TicketSubcategory> GetSubcategoriesByCategoryId(int categoryId)
    {
        return _subcategoryRepo.GetByCategoryId(categoryId);
    }
    // Optional: fetch a single subcategory by its own ID
    public TicketSubcategory GetSubcategoryById(int subcategoryId)
    {
        return _subcategoryRepo.GetById(subcategoryId);
    }
    //public IEnumerable<TicketCategory> GetAllSubcategories()
    //{
    //    return _subcategoryRepo.GetAll();
    //}
}