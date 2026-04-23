using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.service.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TicketCategoryService : ITicketCategoryService
{
    private readonly ITicketCategoryRepository _categoryRepository;

    public TicketCategoryService(ITicketCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public TicketCategory GetCategoryById(int categoryId)
    {
        return _categoryRepository.GetById(categoryId); 
    }
    public IEnumerable<TicketCategory> GetAllCategories()
    {
        return _categoryRepository.GetAll();
    }
}