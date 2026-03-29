using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.src.model.message;

namespace CloudSpritzers1.src.repository
{
    public class MessageDBRepository : DBRepository<int, Message>, IRepository<int, Message>
    {
        protected override Message MapRowToEntity(SqlDataReader reader)
        {
            int senderId = reader.GetInt32(reader.GetOrdinal("sender_id"));
            string text = reader.GetString(reader.GetOrdinal("text"));
            bool isRead = reader.GetBoolean(reader.GetOrdinal("is_read"));

            return new Message(new UserStub(senderId), null, text, isRead);
        }

        protected override int GetEntityId(Message entity)
        {
            return entity.GetId();
        }

        public int Add(Message elem)
        {
            string query = "INSERT INTO Message (sender_id, chat_id, timestamp, text, is_read) " +
                           "VALUES (@senderId, @chatId, @timestamp, @text, @isRead); SELECT SCOPE_IDENTITY();";

            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@senderId", elem.GetSender() is UserStub s ? s.UserId : 0);
            cmd.Parameters.AddWithValue("@chatId", ((IMessage)elem).GetChat() is CloudSpritzers1.src.model.chat.Chat c ? c.ChatId : 0);
            cmd.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow);
            cmd.Parameters.AddWithValue("@text", elem.GetMessage());
            cmd.Parameters.AddWithValue("@isRead", false);

            return Add(cmd, elem);
        }

        public void DeleteById(int id)
        {
            string query = "DELETE FROM Message WHERE message_id = @id";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", id);

            DeleteById(id, cmd);
        }

        public void UpdateById(int id, Message message)
        {
            string query = "UPDATE Message SET text = @text, is_read = @isRead WHERE message_id = @id";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@text", message.GetMessage());
            cmd.Parameters.AddWithValue("@isRead", false);
            cmd.Parameters.AddWithValue("@id", id);

            UpdateById(id, cmd, message);
        }

        public IEnumerable<Message> GetAll()
        {
            string query = "SELECT * FROM Message";
            var cmd = new SqlCommand(query);

            return GetAll(cmd);
        }

        public Message GetById(int id)
        {
            string query = "SELECT * FROM Message WHERE message_id = @id";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", id);

            return GetById(id, cmd) ?? throw new KeyNotFoundException($"Message with id {id} not found.");
        }

        public IEnumerable<Message> GetByChatId(int chatId)
        {
            string query = "SELECT * FROM Message WHERE chat_id = @chatId ORDER BY timestamp";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@chatId", chatId);

            return GetAll(cmd);
        }

        public IEnumerable<Message> GetMessagesSince(int chatId, int firstMessageId)
        {
            string query = "SELECT * FROM Message WHERE chat_id = @chatId AND message_id >= @firstMessageId ORDER BY timestamp";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@firstMessageId", firstMessageId);

            return GetAll(cmd);
        }

        private sealed class UserStub : ISender
        {
            public int UserId { get; }
            public UserStub(int userId) => UserId = userId;
            public string GetName() => string.Empty;
            public string GetEmail() => string.Empty;
        }
    }
}