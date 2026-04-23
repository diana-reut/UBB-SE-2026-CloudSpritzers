using CloudSpritzers1.src.model;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.service.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.repository.interfaces;

namespace CloudSpritzers1.src.service
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetById(int identificationNumber)
        {
            return _userRepository.GetById(identificationNumber);
        }

        public int AddUser(User user)
        {
            return _userRepository.CreateNewEntity(user);
        }

        public void UpdateUserById(int identificationNumber, User userEntity)
        {
            _userRepository.UpdateById(identificationNumber, userEntity);
        }

        public void DeleteUserById(int identificationNumber)
        {
            _userRepository.DeleteById(identificationNumber);
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll().ToList();
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
