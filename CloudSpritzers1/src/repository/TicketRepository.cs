using System.Collections.Generic;
using System;
using CloudSpritzers1.Src.Model.Ticket;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Repository.Database;

namespace CloudSpritzers1.Src.Repository
{
    public class TicketRepository : DatabaseRepository<int, Ticket>, ITicketRepository
    {
        private IUserRepository userRepository = new UserRepository();

        private ITicketCategoryRepository categoryRepository = new TicketCategoryRepository();
        private ITicketSubcategoryRepository subcategoryRepository = new TicketSubcategoryRepository();
        public TicketRepository()
        {
        }

        public Ticket GetById(int id)
        {
            string query = "SELECT * FROM Ticket WHERE ticket_id = @TicketId";
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@TicketId", id);

            Ticket ticket = GetById(id, command);

            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with TicketId {id} was not found.");
            }

            return ticket;
        }

        public IEnumerable<Ticket> GetAll()
        {
            string query = "SELECT * FROM Ticket";
            SqlCommand command = new SqlCommand(query);
            return GetAll(command);
        }

        public int CreateNewEntity(Ticket ticketEntity)
        {
            if (ticketEntity == null)
            {
                throw new ArgumentNullException(nameof(ticketEntity), "Ticket can't be null.");
            }

            string query = @"INSERT INTO Ticket 
                (user_id, status, category_id, subcategory_id, subject, description, created_at, urgency_level) " +
                "OUTPUT INSERTED.Ticket_id " +
                "VALUES (@userId, @status, @categoryId, @subcategoryId, @subject, @description, @creationTimestamp, @urgency)";

            SqlCommand command = new SqlCommand(query);

            command.Parameters.AddWithValue("@userId", ticketEntity.Creator.UserId);
            command.Parameters.AddWithValue("@status", ticketEntity.CurrentStatus.ToString());
            command.Parameters.AddWithValue("@categoryId", ticketEntity.Category.CategoryId);
            command.Parameters.AddWithValue("@subcategoryId", ticketEntity.Subcategory.SubcategoryId);
            command.Parameters.AddWithValue("@subject", ticketEntity.Subject);
            command.Parameters.AddWithValue("@description", ticketEntity.Description);
            command.Parameters.AddWithValue("@creationTimestamp", ticketEntity.CreationTimestamp);
            command.Parameters.AddWithValue("@urgency", ticketEntity.UrgencyLevel.ToString());

            int id = Add(command, ticketEntity);
            return id;
        }

        public void UpdateById(int ticketId, Ticket ticketEntity)
        {
            if (ticketEntity == null)
            {
                throw new ArgumentNullException(nameof(ticketEntity), "Ticket can't be null.");
            }

            string query = @"UPDATE Ticket SET 
                user_id = @userId, 
                status = @status, 
                category_id = @categoryId, 
                subcategory_id = @subcategoryId, 
                subject = @subject, 
                description = @description, 
                created_at = @creationTimestamp, 
                urgency_level = @urgency 
                WHERE ticket_id = @TicketId";

            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@TicketId", ticketId);
            command.Parameters.AddWithValue("@userId", ticketEntity.Creator.UserId);
            command.Parameters.AddWithValue("@status", ticketEntity.CurrentStatus.ToString());
            command.Parameters.AddWithValue("@categoryId", ticketEntity.Category.CategoryId);
            command.Parameters.AddWithValue("@subcategoryId", ticketEntity.Subcategory.SubcategoryId);
            command.Parameters.AddWithValue("@subject", ticketEntity.Subject);
            command.Parameters.AddWithValue("@description", ticketEntity.Description);
            command.Parameters.AddWithValue("@creationTimestamp", ticketEntity.CreationTimestamp);
            command.Parameters.AddWithValue("@urgency", ticketEntity.UrgencyLevel.ToString());

            UpdateById(ticketId, command, ticketEntity);
        }

        public void DeleteById(int ticketId)
        {
            string query = "DELETE FROM Ticket WHERE ticket_id = @TicketId";
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@TicketId", ticketId);

            DeleteById(ticketId, command);
        }

        protected override Ticket MapRowToEntity(SqlDataReader reader)
        {
            int ticketId = reader.GetInt32(reader.GetOrdinal("ticket_id"));
            int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
            TicketStatusEnum status = Enum.Parse<TicketStatusEnum>(reader.GetString(reader.GetOrdinal("status")), ignoreCase: true);
            int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
            int subcategoryId = reader.GetInt32(reader.GetOrdinal("subcategory_id"));
            string subject = reader.GetString(reader.GetOrdinal("subject"));
            string description = reader.GetString(reader.GetOrdinal("description"));
            DateTime creationTimestamp = reader.GetDateTime(reader.GetOrdinal("created_at"));
            TicketUrgencyLevelEnum urgency = Enum.Parse<TicketUrgencyLevelEnum>(reader.GetString(reader.GetOrdinal("urgency_level")), ignoreCase: true);

            TicketCategory category = categoryRepository.GetById(categoryId);
            TicketSubcategory subcategory = subcategoryRepository.GetById(subcategoryId);
            User creator = userRepository.GetById(userId);

            return new Ticket(ticketId, creator, status, category, subcategory, subject, description, creationTimestamp, urgency);
        }

        protected override int GetEntityId(Ticket ticketEntity)
        {
            return ticketEntity.TicketId;
        }
    }
}