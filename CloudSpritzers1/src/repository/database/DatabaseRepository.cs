using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.database;
using Microsoft.Data.SqlClient;
namespace CloudSpritzers1.src.repository.database
{

    public abstract class DatabaseRepository<TKey, TEntity>
        where TEntity : class
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TEntity> _entityCache = new();

        protected SqlConnection CreateConnection() => DatabaseConnectionHandler.Instance.CreateConnection();
        protected abstract TEntity MapRowToEntity(SqlDataReader reader);
        protected abstract TKey GetEntityId(TEntity entity);

        /// <summary>
        /// Gets an entity by its id OR returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        protected TEntity GetById(TKey id, SqlCommand command)
        {
            if (_entityCache.TryGetValue(id, out var cached))
                return cached;

            var entity = ExecuteQuerySingle(command);
            if (entity != null)
                _entityCache[id] = entity;

            return entity;
        }

        protected IEnumerable<TEntity> GetAll(SqlCommand command)
        {
            var results = ExecuteQueryMany(command).ToList();
            foreach (var entity in results)
                _entityCache[GetEntityId(entity)] = entity;
            return results;
        }

        protected TKey Add(SqlCommand command, TEntity entity)
        {
            var id = ExecuteScalar<TKey>(command);
            _entityCache[id] = entity;
            return id;
        }

        protected void DeleteById(TKey id, SqlCommand command)
        {
            ExecuteNonQuery(command);
            _entityCache.Remove(id);
        }

        protected void UpdateById(TKey id, SqlCommand command, TEntity entity)
        {
            ExecuteNonQuery(command);
            _entityCache[id] = entity;
        }


        //NOTE : If testing becomes a requirement, override the following query methods to work over something in memory.

        /// <summary>
        /// Returns one entity matching the query. If no matching row in db is found => null!
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected virtual TEntity ExecuteQuerySingle(SqlCommand command)
        {
            using var connection = CreateConnection();
            command.Connection = connection;
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? MapRowToEntity(reader) : null;
        }

        protected virtual IEnumerable<TEntity> ExecuteQueryMany(SqlCommand command)
        {
            using var connection = CreateConnection();
            command.Connection = connection;
            connection.Open();
            using var reader = command.ExecuteReader();
            var results = new List<TEntity>();
            while (reader.Read())
                results.Add(MapRowToEntity(reader));
            return results;
        }

        protected virtual void ExecuteNonQuery(SqlCommand command)
        {
            using var connection = CreateConnection();
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();
        }

        protected virtual T ExecuteScalar<T>(SqlCommand command)
        {
            using var connection = CreateConnection();
            command.Connection = connection;
            connection.Open();
            return (T)command.ExecuteScalar();
        }


        protected void InvalidateCache() => _entityCache.Clear();
        protected void InvalidateCacheEntry(TKey id) => _entityCache.Remove(id);
    }
}