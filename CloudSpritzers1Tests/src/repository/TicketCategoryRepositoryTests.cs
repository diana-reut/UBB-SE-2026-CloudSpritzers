using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1Tests.Src.MockClasses;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass]
    public class TicketCategoryRepositoryTests
    {
        private ITicketCategoryRepository _categoryRepository;

        [TestInitialize]
        public void Setup()
        {
            
            _categoryRepository = new InMemoryTicketCategoryRepository();
        }

        [TestMethod]
        public void GetById_WithExistingId_Succeeds()
        {
            
            var category = new TicketCategory(1, "Technical Support", TicketUrgencyLevelEnum.HIGH);
            ((InMemoryTicketCategoryRepository)_categoryRepository).CreateNewEntity(category);

            
            var result = _categoryRepository.GetById(1);

            
            Assert.AreEqual(category.CategoryName, result.CategoryName);
            Assert.AreEqual(category.CategoryUrgencyLevel, result.CategoryUrgencyLevel);
            Assert.AreEqual(category, result);
        }

        [TestMethod]
        public void GetById_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            
            Assert.ThrowsExactly<KeyNotFoundException>(() => _categoryRepository.GetById(999));
        }

        [TestMethod]
        public void GetAll_ReturnsAllEntities()
        {
            
            var category1 = new TicketCategory(1, "IT", TicketUrgencyLevelEnum.MEDIUM);
            var category2 = new TicketCategory(2, "HR", TicketUrgencyLevelEnum.LOW);

            var repo = (InMemoryTicketCategoryRepository)_categoryRepository;
            repo.CreateNewEntity(category1);
            repo.CreateNewEntity(category2);

            
            var result = _categoryRepository.GetAll().ToList();
            
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result.Select(c => c.CategoryName).ToList(), "IT");
        }

        [TestMethod]
        public void CreateNewEntity_WithDuplicateId_ThrowsArgumentException()
        {
            
            var category = new TicketCategory(1, "IT", TicketUrgencyLevelEnum.MEDIUM);
            var repo = (InMemoryTicketCategoryRepository)_categoryRepository;
            repo.CreateNewEntity(category);

            
            Assert.ThrowsExactly<ArgumentException>(() => repo.CreateNewEntity(category));
        }



    }
}