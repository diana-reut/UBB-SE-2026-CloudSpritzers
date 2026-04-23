using System.Collections.Generic;
using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1.Src.Repository.Interfaces
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