using CloudSpritzers1.src.model;
using CloudSpritzers1.src.repository.interfaces;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.src.repository.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.repository
{
    public class UserRepository : DatabaseRepository<int, User>, IRepository<int, User>, IUserRepository
    {
        public int CreateNewEntity(User userEntity)
        {
            if (userEntity == null)
                throw new ArgumentNullException(nameof(userEntity), "User cannot be null.");

            string insertQuery = "INSERT INTO [User] " +
                "(name, email) " +
                "OUTPUT INSERTED.user_id " +
                "VALUES (@name, @email)";

            SqlCommand sqlCommand = new SqlCommand(insertQuery);


            sqlCommand.Parameters.AddWithValue("@name", userEntity.RetrieveConfiguredDisplayFullNameForBot());
            sqlCommand.Parameters.AddWithValue("@email", userEntity.RetrieveConfiguredEmailAddressForBotContact());


            int generatedIdentificationNumber = base.Add(sqlCommand, userEntity);
            return generatedIdentificationNumber;
        }

        public void DeleteById(int identificationNumber)
        {
            string deleteQuery = "DELETE FROM [User] WHERE user_id = @id";
            SqlCommand sqlCommand = new SqlCommand(deleteQuery);
            sqlCommand.Parameters.AddWithValue("@id", identificationNumber);

            base.DeleteById(identificationNumber, sqlCommand);
        }

        public IEnumerable<User> GetAll()
        {
            string selectAllQuery = "SELECT * FROM [User]";
            SqlCommand command = new SqlCommand(selectAllQuery);
            return base.GetAll(command);
        }

        public User GetById(int identificationNumber)
        {
            string selectByIdQuery = "SELECT * FROM [User] WHERE user_id = @id";
            SqlCommand sqlCommand = new SqlCommand(selectByIdQuery);
            sqlCommand.Parameters.AddWithValue("@id", identificationNumber);

            User foundUser = base.GetById(identificationNumber, sqlCommand);

            if (foundUser == null)
                throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");

            return foundUser;
        }

        public void UpdateById(int identificationNumber, User userEntity)
        {
            if (userEntity == null)
                throw new ArgumentNullException(nameof(userEntity), "User cannot be null.");

            string updateQuery = "UPDATE [User] SET " +
                "name = @name, " +
                "email = @email " +
                "WHERE user_id = @id";

            SqlCommand sqlCommand = new SqlCommand(updateQuery);


            sqlCommand.Parameters.AddWithValue("@id", identificationNumber);
            sqlCommand.Parameters.AddWithValue("@name", userEntity.RetrieveConfiguredDisplayFullNameForBot());
            sqlCommand.Parameters.AddWithValue("@email", userEntity.RetrieveConfiguredEmailAddressForBotContact());

            

            base.UpdateById(identificationNumber, sqlCommand, userEntity);
        }

        protected override int GetEntityId(User userEntity)
        {
            return userEntity.UserId;
        }

        protected override User MapRowToEntity(SqlDataReader sqlDataReader)
        {
            int userIdentificationNumber = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("user_id"));
            string userFullName = sqlDataReader.GetString(sqlDataReader.GetOrdinal("name"));
            string userEmailAddress = sqlDataReader.GetString(sqlDataReader.GetOrdinal("email"));

            return new User(userIdentificationNumber, userFullName, userEmailAddress);
        }
    }
}
