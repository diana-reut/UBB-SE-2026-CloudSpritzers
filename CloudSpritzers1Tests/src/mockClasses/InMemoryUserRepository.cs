using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Repository.Interfaces;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryUserRepository : IUserRepository
    {
        private List<User> users = new List<User>();

        public int CreateNewEntity(User userEntity)
        {
            if (userEntity == null) throw new ArgumentNullException();
            users.Add(userEntity);
            return userEntity.RetrieveUniqueDatabaseIdentifierForBot();
        }

        public void DeleteById(int id)
        {
            var user = users.FirstOrDefault(u => u.RetrieveUniqueDatabaseIdentifierForBot() == id);
            if (user == null) throw new KeyNotFoundException();
            users.Remove(user);
        }

        public IEnumerable<User> GetAll() => users;

        public User GetById(int id)
        {
            var user = users.FirstOrDefault(u => u.RetrieveUniqueDatabaseIdentifierForBot() == id);
            if (user == null) throw new KeyNotFoundException();
            return user;
        }

        public void UpdateById(int id, User userEntity)
        {
            var index = users.FindIndex(u => u.RetrieveUniqueDatabaseIdentifierForBot() == id);
            if (index == -1) throw new KeyNotFoundException();
            users[index] = userEntity;
        }
    }
}