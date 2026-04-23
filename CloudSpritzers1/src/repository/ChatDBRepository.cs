using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Data;
using Microsoft.Data.SqlClient;

using CloudSpritzers1.Src.Model.Chats;

namespace CloudSpritzers1.Src.Repository.Database
{
	public class ChatDatabaseRepository : DatabaseRepository<int, Chat>, IRepository<int, Chat>
	{
        /// <summary>
        /// Helper method to map a reader row to a Chat object to respect DRY
        /// </summary>
        /// <param name="sqlDataReaderContainingDatabaseRowData"> SqlDataReader </param>
        /// <returns> Chat object </returns>
        protected override Chat MapRowToEntity(SqlDataReader sqlDataReaderContainingDatabaseRowData)
        {
            int uniqueDatabaseIdentifierForCurrentChat = sqlDataReaderContainingDatabaseRowData.GetInt32(sqlDataReaderContainingDatabaseRowData.GetOrdinal("chat_id"));
            int uniqueDatabaseIdentifierForTheUserAssociatedWithThisChat = sqlDataReaderContainingDatabaseRowData.GetInt32(sqlDataReaderContainingDatabaseRowData.GetOrdinal("user_id"));
            string stringRepresentationOfTheChatStatusRetrievedFromDatabase = sqlDataReaderContainingDatabaseRowData.GetString(sqlDataReaderContainingDatabaseRowData.GetOrdinal("status"));
            ChatStatus parsedChatStatusEnumerationValue = (ChatStatus)Enum.Parse(typeof(ChatStatus), stringRepresentationOfTheChatStatusRetrievedFromDatabase);

            return new Chat(uniqueDatabaseIdentifierForCurrentChat, uniqueDatabaseIdentifierForTheUserAssociatedWithThisChat, parsedChatStatusEnumerationValue);
        }

        protected override int GetEntityId(Chat specificChatEntity)
        {
            return specificChatEntity.ChatId;
        }

        public int CreateNewEntity(Chat incomingChatEntityToBeSaved)
        {
            string sqlQueryStringForInsertingNewChatIntoDatabase = "INSERT INTO Chat (user_id, status) " +
                               "VALUES (@userId, @status); SELECT CAST( SCOPE_IDENTITY() AS INT);";

            var sqlCommandObjectForExecutingInsertQuery = new SqlCommand(sqlQueryStringForInsertingNewChatIntoDatabase);
            sqlCommandObjectForExecutingInsertQuery.Parameters.AddWithValue("@userId", Convert.ToInt32(incomingChatEntityToBeSaved.UserId));
            sqlCommandObjectForExecutingInsertQuery.Parameters.AddWithValue("@status", incomingChatEntityToBeSaved.Status.ToString());

            return Add(sqlCommandObjectForExecutingInsertQuery, incomingChatEntityToBeSaved);
        }

        public void DeleteById(int identifierForChatToBeDeleted)
        {
            string sqlQueryStringForDeletingSpecificChatFromDatabase = "DELETE FROM Chat WHERE chat_id = @id";
            var sqlCommandObjectForExecutingDeleteQuery = new SqlCommand(sqlQueryStringForDeletingSpecificChatFromDatabase);
            sqlCommandObjectForExecutingDeleteQuery.Parameters.AddWithValue("@id", identifierForChatToBeDeleted);

            DeleteById(identifierForChatToBeDeleted, sqlCommandObjectForExecutingDeleteQuery);
        }

        public void UpdateById(int identifierForChatToBeUpdated, Chat updatedChatEntityData)
        {
            string sqlQueryStringForUpdatingSpecificChatInDatabase = "UPDATE Chat SET user_id = @userId, status = @status WHERE chat_id = @id";
            var sqlCommandObjectForExecutingUpdateQuery = new SqlCommand(sqlQueryStringForUpdatingSpecificChatInDatabase);
            sqlCommandObjectForExecutingUpdateQuery.Parameters.AddWithValue("@userId", updatedChatEntityData.UserId);
            sqlCommandObjectForExecutingUpdateQuery.Parameters.AddWithValue("@status", updatedChatEntityData.Status.ToString());
            sqlCommandObjectForExecutingUpdateQuery.Parameters.AddWithValue("@id", identifierForChatToBeUpdated);

            UpdateById(identifierForChatToBeUpdated, sqlCommandObjectForExecutingUpdateQuery, updatedChatEntityData);
        }

        public IEnumerable<Chat> GetAll()
        {
            string sqlQueryStringForRetrievingAllChatsFromDatabase = "SELECT * FROM Chat";
            var sqlCommandObjectForExecutingSelectAllQuery = new SqlCommand(sqlQueryStringForRetrievingAllChatsFromDatabase);

            return GetAll(sqlCommandObjectForExecutingSelectAllQuery);
        }

        public Chat GetById(int identifierForRequestedChat)
        {
            string sqlQueryStringForRetrievingSpecificChatFromDatabase = "SELECT * FROM Chat WHERE chat_id = @id";
            var sqlCommandObjectForExecutingSelectByIdQuery = new SqlCommand(sqlQueryStringForRetrievingSpecificChatFromDatabase);
            sqlCommandObjectForExecutingSelectByIdQuery.Parameters.AddWithValue("@id", identifierForRequestedChat);

            return GetById(identifierForRequestedChat, sqlCommandObjectForExecutingSelectByIdQuery) ?? throw new KeyNotFoundException($"Chat with id {identifierForRequestedChat} not found.");
        }
    }
}