using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DotNetEnv;
using Microsoft.Data.SqlClient;
namespace CloudSpritzers1.Src.Repository.Database
{
    public class DatabaseConnectionHandler
    {
        private static readonly DatabaseConnectionHandler DatabaseInstance = new DatabaseConnectionHandler();
        public static DatabaseConnectionHandler Instance => DatabaseInstance;

        private readonly string connectionString;
        // https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-connection-pooling
        // Seems like ado.net pools connections by default, sorry Maria
        public SqlConnection CreateConnection() => new SqlConnection(connectionString);
        private DatabaseConnectionHandler()
        {
            connectionString = InitializeConnectionString();
        }

        private string InitializeConnectionString()
        {
            string serverAddress = Env.GetString("DB_SERVER");
            if (serverAddress == null)
            {
                throw new DatabaseConnectionException("DB_SERVER environment variable is not set.");
            }

            string databaseName = Env.GetString("DB_NAME");
            if (databaseName == null)
            {
                throw new DatabaseConnectionException("DB_NAME environment variable is not set.");
            }
            string userName = Env.GetString("DB_USER");
            if (userName == null)
            {
                throw new DatabaseConnectionException("DB_USER environment variable is not set.");
            }
            string userPassword = Env.GetString("DB_PASS");
            if (userPassword == null)
            {
                throw new DatabaseConnectionException("DB_PASS environment variable is not set.");
            }

            string connectionString = $"Server={serverAddress};Database={databaseName};User Id={userName};Password={userPassword};TrustServerCertificate=True;";
            return connectionString;
        }
    }
}
