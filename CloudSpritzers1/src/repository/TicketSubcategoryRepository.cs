using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.database;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TicketSubcategoryRepository : DatabaseRepository<int, TicketSubcategory> , ITicketSubcategoryRepository
{
    public IEnumerable<TicketSubcategory> GetAll()
    {
        string query = "SELECT * FROM TicketSubcategory";
        SqlCommand command = new SqlCommand(query);
        return base.GetAll(command);
    }

    public TicketSubcategory GetById(int subcategoryId)
    {
        string query = "SELECT * FROM TicketSubcategory WHERE subcategory_id = @id";
        SqlCommand command = new SqlCommand(query);
        command.Parameters.AddWithValue("@id", subcategoryId);

        return base.GetAll(command).FirstOrDefault()
               ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
    }
    public IEnumerable<TicketSubcategory> GetByCategoryId(int categoryId)
    {
        string query = "SELECT * FROM TicketSubcategory WHERE category_id = @categoryId";
        SqlCommand command = new SqlCommand(query);
        command.Parameters.AddWithValue("@categoryId", categoryId);

        return base.GetAll(command);
    }

    protected override TicketSubcategory MapRowToEntity(SqlDataReader reader)
    {
        int subcategoryId = reader.GetInt32(reader.GetOrdinal("subcategory_id"));
        string subcategoryName = reader.GetString(reader.GetOrdinal("name"));
        int externalReferenceId = reader.GetInt32(reader.GetOrdinal("external_id"));

        int parentCategoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
        var categoryRepository = new TicketCategoryRepository();
        TicketCategory parentCategory = categoryRepository.GetById(parentCategoryId);

        return new TicketSubcategory(subcategoryId, subcategoryName, externalReferenceId, parentCategory);
    }

    protected override int GetEntityId(TicketSubcategory subcategoryEntity) => subcategoryEntity.SubcategoryId;
}