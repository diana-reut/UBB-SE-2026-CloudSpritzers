using CloudSpritzers1.src.model;
using System.Collections.Generic;

namespace CloudSpritzers1.src.repository.interfaces
{
    public interface IUserRepository
    {
        int CreateNewEntity(User userEntity);
        void DeleteById(int identificationNumber);
        IEnumerable<User> GetAll();
        User GetById(int identificationNumber);
        void UpdateById(int identificationNumber, User userEntity);
    }
}