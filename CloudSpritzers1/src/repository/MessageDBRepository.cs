using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.model.chat;

namespace CloudSpritzers1.src.repository.database
{
    public class MessageDatabaseRepository : DatabaseRepository<int, Message>, IRepository<int, Message>
    {
        protected override Message MapRowToEntity(SqlDataReader reader)
        {
            int messageId = reader.GetInt32(reader.GetOrdinal("message_id"));
            int chatId = reader.GetInt32(reader.GetOrdinal("chat_id"));
            int senderId = reader.GetInt32(reader.GetOrdinal("sender_id"));
            string text = reader.GetString(reader.GetOrdinal("text"));
            ///DateTimeOffset timestamp = reader.GetDateTimeOffset(reader.GetOrdinal("timestamp"));
            // Read as DateTime, then convert to DateTimeOffset
            DateTime dbDt = reader.GetDateTime(reader.GetOrdinal("timestamp"));
            DateTimeOffset timestamp = new DateTimeOffset(dbDt);

            var senderStub = new SenderStub(senderId);
            var chatStub = new ChatStub(chatId);

            return new Message(messageId, senderStub, chatStub, text, timestamp);
        }

        protected override int GetEntityId(Message entity) => entity.GetId();

        public int CreateNewEntity(Message elem)
        {
            const string query =
                "INSERT INTO Message (sender_id, chat_id, timestamp, text, is_read) " +
                "VALUES (@senderId, @chatId, @timestamp, @text, @isRead); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@senderId", elem.GetSender().RetrieveUniqueDatabaseIdentifierForBot());
            cmd.Parameters.AddWithValue("@chatId", ((IMessage)elem).GetChat().ChatId);
            cmd.Parameters.AddWithValue("@timestamp", DateTimeOffset.UtcNow);
            cmd.Parameters.AddWithValue("@text", elem.GetMessage());
            cmd.Parameters.AddWithValue("@isRead", false);

            return Add(cmd, elem);
        }

        public void DeleteById(int id)
        {
            const string query = "DELETE FROM Message WHERE message_id = @id";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", id);

            DeleteById(id, cmd);
        }

        public void UpdateById(int id, Message message)
        {
            const string query = "UPDATE Message SET text = @text WHERE message_id = @id";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@text", message.GetMessage());
            cmd.Parameters.AddWithValue("@id", id);

            UpdateById(id, cmd, message);
        }

        public IEnumerable<Message> GetAll()
        {
            const string query = "SELECT * FROM Message";
            return GetAll(new SqlCommand(query));
        }

        public Message GetById(int id)
        {
            const string query = "SELECT * FROM Message WHERE message_id = @id";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", id);

            return GetById(id, cmd)
                ?? throw new KeyNotFoundException($"Message with id {id} not found.");
        }

        public IEnumerable<Message> GetByChatId(int chatId)
        {
            const string query =
                "SELECT * FROM Message WHERE chat_id = @chatId ORDER BY timestamp ASC";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@chatId", chatId);

            return GetAll(cmd);
        }

        public IEnumerable<Message> GetMessagesSince(int chatId, int firstMessageId)
        {
            const string query =
                "SELECT * FROM Message " +
                "WHERE chat_id = @chatId AND message_id >= @firstMessageId " +
                "ORDER BY timestamp ASC";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@chatId", chatId);
            cmd.Parameters.AddWithValue("@firstMessageId", firstMessageId);

            return GetAll(cmd);
        }

        public void MarkAsRead(int messageId)
        {
            const string query = "UPDATE Message SET is_read = 1 WHERE message_id = @id";
            var cmd = new SqlCommand(query);
            cmd.Parameters.AddWithValue("@id", messageId);

            ExecuteNonQuery(cmd);
            InvalidateCacheEntry(messageId);
        }


        // TODO: I swear I wanted to remove stubs, not end more. I hope God and Mihai will forgive me, at least Mihai.
        private sealed class SenderStub : ISender
        {
            private readonly int _id;
            public SenderStub(int id) => _id = id;
            public int RetrieveUniqueDatabaseIdentifierForBot() => _id;
            public string RetrieveConfiguredDisplayFullNameForBot() => string.Empty;
            public string RetrieveConfiguredEmailAddressForBotContact() => string.Empty;
        }


        private sealed class ChatStub : Chat
        {
            public ChatStub(int chatId) : base(chatId, userId: 0, ChatStatus.Active) { }
        }
    }
}