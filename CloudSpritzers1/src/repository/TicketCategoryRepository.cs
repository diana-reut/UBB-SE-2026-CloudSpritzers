using CloudSpritzers1.src.model.ticket;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TicketCategoryRepository : DBRepository<int, TicketCategory>
{
    public IEnumerable<TicketCategory> GetAll()
    {
        string query = "SELECT * FROM TicketCategory";
        SqlCommand command = new SqlCommand(query);
        return base.GetAll(command);
    }

    public TicketCategory GetById(int id)
    {
        string query = "SELECT * FROM TicketCategory WHERE category_id = @id";
        SqlCommand command = new SqlCommand(query);
        command.Parameters.AddWithValue("@id", id);
        return base.GetById(id, command);
    }

    protected override TicketCategory MapRowToEntity(SqlDataReader reader)
    {
        int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
        string name = reader.GetString(reader.GetOrdinal("name"));
        //UrgencyLevelEnum urgency = UrgencyLevelEnum.LOW;
        string urgencyStr = reader.GetString(reader.GetOrdinal("urgency_level"));

        if (!Enum.TryParse<UrgencyLevelEnum>(urgencyStr, true, out var urgency))
        {
            urgency = UrgencyLevelEnum.LOW; 
        }
        return new TicketCategory(categoryId, name, urgency);
    }

    protected override int GetEntityId(TicketCategory entity) => entity.CategoryId;
}