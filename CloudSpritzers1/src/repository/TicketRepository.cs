using CloudSpritzers1.src.model.ticket;
using System.Collections.Generic;
using System;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.faq;

namespace CloudSpritzers1.src.repository
{
    public class TicketRepository : DBRepository<int, Ticket>, IRepository<int, Ticket>
    {

        private UserRepository _userRepository = new UserRepository();

        private TicketCategoryRepository _categoryRepository = new TicketCategoryRepository();
        private TicketSubcategoryRepository _subcategoryRepository = new TicketSubcategoryRepository();
        public TicketRepository() { }

        public Ticket GetById(int id)
        {
            string query = "SELECT * FROM Ticket WHERE ticket_id = @id";
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@id", id);

            Ticket ticket = base.GetById(id, command);

            if (ticket == null)
                throw new KeyNotFoundException($"Ticket with id {id} was not found.");

            return ticket;
        }

        public IEnumerable<Ticket> GetAll()
        {
            string query = "SELECT * FROM Ticket";
            SqlCommand command = new SqlCommand(query);
            return base.GetAll(command);

        }

        public int Add(Ticket elem)
        {
            if (elem == null)
                throw new ArgumentNullException(nameof(elem), "Ticket can't be null.");

            string query = @"INSERT INTO Ticket 
                (user_id, status, category_id, subcategory_id, subject, description, created_at, urgency_level) " +
                "OUTPUT INSERTED.ticket_id " +
                "VALUES (@userId, @status, @categoryId, @subcategoryId, @subject, @description, @createdAt, @urgency)";

            SqlCommand command = new SqlCommand(query);

            command.Parameters.AddWithValue("@userId", elem.User.UserId);
            command.Parameters.AddWithValue("@status", elem.Status.ToString());
            command.Parameters.AddWithValue("@categoryId", elem.Category.CategoryId);
            command.Parameters.AddWithValue("@subcategoryId", elem.Subcategory.SubcategoryId);
            command.Parameters.AddWithValue("@subject", elem.Subject);
            command.Parameters.AddWithValue("@description", elem.Description);
            command.Parameters.AddWithValue("@createdAt", elem.CreatedAt);
            command.Parameters.AddWithValue("@urgency", elem.UrgencyLevel.ToString());

            int id = base.Add(command, elem);
            return id;
        }

        public void UpdateById(int id, Ticket elem)
        {
            if (elem == null)
                throw new ArgumentNullException(nameof(elem), "Ticket can't be null.");

            string query = @"UPDATE Ticket SET 
                user_id = @userId, 
                status = @status, 
                category_id = @categoryId, 
                subcategory_id = @subcategoryId, 
                subject = @subject, 
                description = @description, 
                created_at = @createdAt, 
                urgency_level = @urgency 
                WHERE ticket_id = @id";

            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@userId", elem.User.UserId);
            command.Parameters.AddWithValue("@status", elem.Status.ToString());
            command.Parameters.AddWithValue("@categoryId", elem.Category.CategoryId);
            command.Parameters.AddWithValue("@subcategoryId", elem.Subcategory.SubcategoryId);
            command.Parameters.AddWithValue("@subject", elem.Subject);
            command.Parameters.AddWithValue("@description", elem.Description);
            command.Parameters.AddWithValue("@createdAt", elem.CreatedAt);
            command.Parameters.AddWithValue("@urgency", elem.UrgencyLevel.ToString());

            base.UpdateById(id, command, elem);
        }

        public void DeleteById(int id)
        {
            string query = "DELETE FROM Ticket WHERE ticket_id = @id";
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@id", id);

            base.DeleteById(id, command);
        }

        protected override Ticket MapRowToEntity(SqlDataReader reader)
        {
            int ticketId = reader.GetInt32(reader.GetOrdinal("ticket_id"));
            int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
            StatusEnum status = Enum.Parse<StatusEnum>(reader.GetString(reader.GetOrdinal("status")),ignoreCase: true);
            int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
            int subcategoryId = reader.GetInt32(reader.GetOrdinal("subcategory_id"));
            string subject = reader.GetString(reader.GetOrdinal("subject"));
            string description = reader.GetString(reader.GetOrdinal("description"));
            DateTime createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
            UrgencyLevelEnum urgency = Enum.Parse<UrgencyLevelEnum>(reader.GetString(reader.GetOrdinal("urgency_level")),ignoreCase:true);

            TicketCategory category = _categoryRepository.GetById(categoryId);
            TicketSubcategory subcategory = _subcategoryRepository.GetById(subcategoryId);
            User user = _userRepository.GetById(userId);

            return new Ticket(ticketId, user, status, category, subcategory, subject, description, createdAt, urgency);
        }

        protected override int GetEntityId(Ticket entity)
        {
            return entity.TicketId;
        }

    }
}