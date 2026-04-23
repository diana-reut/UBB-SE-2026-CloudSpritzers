using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1Tests.Src.MockClasses;
using System.Collections.Generic;
using System.Linq;


namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass]
    public class UserRepositoryTests
    {
        private IUserRepository? _userRepository;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = new InMemoryUserRepository();
        }

        [TestMethod]
        public void GetById_ExistingUser_ReturnsCorrectUser()
        {
            var user = new User(1, "Test Name", "test@email.com");
            _userRepository.CreateNewEntity(user);

            var result = _userRepository.GetById(1);

            Assert.AreEqual(user.RetrieveConfiguredDisplayFullNameForBot(), result.RetrieveConfiguredDisplayFullNameForBot());
            Assert.AreEqual(user.RetrieveUniqueDatabaseIdentifierForBot(), result.RetrieveUniqueDatabaseIdentifierForBot());
        }

        [TestMethod]
        public void GetById_NonExistingId_ThrowsKeyNotFoundException()
        {
            var exception = Assert.ThrowsExactly<KeyNotFoundException>(() =>
                _userRepository!.GetById(999));

            StringAssert.Contains("The given key was not present in the dictionary.", exception.Message);
        }

        [TestMethod]
        public void Add_NullUser_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _userRepository!.CreateNewEntity(null!));
        }

        [TestMethod]
        public void GetAll_ReturnsAllUsers()
        {
            _userRepository.CreateNewEntity(new User(1, "User1", "u1@test.com"));
            _userRepository.CreateNewEntity(new User(2, "User2", "u2@test.com"));

            var results = _userRepository.GetAll().ToList();

            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void UpdateById_ExistingUser_UpdatesCorrectly()
        {
            var user = new User(1, "Old Name", "old@test.com");
            _userRepository!.CreateNewEntity(user);
            var updatedUser = new User(1, "New Name", "new@test.com");

            _userRepository.UpdateById(1, updatedUser);
            var result = _userRepository.GetById(1);

            Assert.AreEqual("New Name", result.RetrieveConfiguredDisplayFullNameForBot());
            Assert.AreEqual("new@test.com", result.RetrieveConfiguredEmailAddressForBotContact());
        }

        [TestMethod]
        public void DeleteById_ExistingUser_DecreasesCount()
        {
            _userRepository!.CreateNewEntity(new User(1, "User1", "u1@test.com"));

            _userRepository.DeleteById(1);
            var results = _userRepository.GetAll().ToList();

            Assert.AreEqual(0, results.Count);
        }
    }
}