using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.Repository.Interfaces;

namespace CloudSpritzers1.Src.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public User GetById(int identificationNumber)
        {
            return userRepository.GetById(identificationNumber);
        }

        public int AddUser(User user)
        {
            return userRepository.CreateNewEntity(user);
        }

        public void UpdateUserById(int identificationNumber, User userEntity)
        {
            userRepository.UpdateById(identificationNumber, userEntity);
        }

        public void DeleteUserById(int identificationNumber)
        {
            userRepository.DeleteById(identificationNumber);
        }

        public List<User> GetAllUsers()
        {
            return userRepository.GetAll().ToList();
        }

        public void CreateNewUser(int identificationNumber, string fullName, string emailAddress)
        {
            User user = new User(identificationNumber, fullName, emailAddress);
            ValidateUserIntegrity(user);
            AddUser(user);
        }

        public void ValidateUserIntegrity(User userEntity)
        {
            ArgumentNullException.ThrowIfNull(userEntity);
            if (this.GetAllUsers().Contains(userEntity))
            {
                throw new ArgumentException("User already exists");
            }
            if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredDisplayFullNameForBot()))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }
            if (string.IsNullOrEmpty(userEntity.RetrieveConfiguredEmailAddressForBotContact()))
            {
                throw new ArgumentException("Email cannot be null or empty");
            }
        }
    }
}
