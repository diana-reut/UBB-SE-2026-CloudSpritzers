using System.Collections.Generic;
using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1.Src.Service.Interfaces
{
    public interface IUserService
    {
        User GetById(int identificationNumber);
        int AddUser(User userEntity);
        void UpdateUserById(int identificationNumber, User userEntity);
        void DeleteUserById(int identificationNumber);
        List<User> GetAllUsers();
        void CreateNewUser(int identificationNumber, string fullName, string emailAddress);
        void ValidateUserIntegrity(User userEntity);
    }
}