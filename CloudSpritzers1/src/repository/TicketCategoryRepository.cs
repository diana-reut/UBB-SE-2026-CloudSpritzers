using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Repository.Database;
using CloudSpritzers1.Src.Repository.Database;
using Microsoft.Data.SqlClient;

using CloudSpritzers1.Src.Repository;

public class TicketCategoryRepository : DatabaseRepository<int, TicketCategory>, ITicketCategoryRepository
{
    public IEnumerable<TicketCategory> GetAll()
    {
        string query = "SELECT * FROM TicketCategory";
        SqlCommand command = new SqlCommand(query);
        return GetAll(command);
    }

    public TicketCategory GetById(int categoryId)
    {
        string query = "SELECT * FROM TicketCategory WHERE category_id = @id";
        SqlCommand command = new SqlCommand(query);
        command.Parameters.AddWithValue("@id", categoryId);
        return GetById(categoryId, command);
    }

    protected override TicketCategory MapRowToEntity(SqlDataReader reader)
    {
        int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
        string categoryName = reader.GetString(reader.GetOrdinal("name"));
        string urgencyLevelString = reader.GetString(reader.GetOrdinal("urgency_level"));

        if (!Enum.TryParse<TicketUrgencyLevelEnum>(urgencyLevelString, true, out var urgencyLevel))
        {
            urgencyLevel = TicketUrgencyLevelEnum.LOW;
        }
        return new TicketCategory(categoryId, categoryName, urgencyLevel);
    }

    protected override int GetEntityId(TicketCategory categoryEntity) => categoryEntity.CategoryId;
}