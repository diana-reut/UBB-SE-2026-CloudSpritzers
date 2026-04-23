using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1Tests.Src.MockClasses;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass]
    public class TicketSubcategoryRepositoryTests
    {
        private ITicketSubcategoryRepository _subcategoryRepository;
        private TicketCategory _testCategory;

        [TestInitialize]
        public void Setup()
        {
            _subcategoryRepository = new InMemoryTicketSubcategoryRepository();
            _testCategory = new TicketCategory(1, "Facultate", TicketUrgencyLevelEnum.HIGH);
        }

        [TestMethod]
        public void GetById_WhenSubcategoryExists_ReturnsCorrectEntity()
        {
            var subcategory = new TicketSubcategory(10, "ISS", 501, _testCategory);
            ((InMemoryTicketSubcategoryRepository)_subcategoryRepository).CreateNewEntity(subcategory);

            var result = _subcategoryRepository.GetById(10);

            Assert.AreEqual("ISS", result.SubcategoryName);
            Assert.AreEqual(501, result.SubcategoryExternalReferenceId);
            Assert.AreEqual(_testCategory.CategoryId, result.ParentCategory.CategoryId);
            Assert.AreEqual(result, subcategory);
        }

        [TestMethod]
        public void GetById_WhenIdDoesNotExist_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _subcategoryRepository.GetById(999));
        }

        [TestMethod]
        public void GetByCategoryId_ReturnsOnlySubcategoriesForThatCategory()
        {
            var otherCategory = new TicketCategory(2, "FMI", TicketUrgencyLevelEnum.LOW);

            var sub1 = new TicketSubcategory(1, "ISS", 101, _testCategory);
            var sub2 = new TicketSubcategory(2, "MPP", 102, _testCategory);
            var sub3 = new TicketSubcategory(3, "WEB", 103, otherCategory);

            var repo = (InMemoryTicketSubcategoryRepository)_subcategoryRepository;//so that i can create instances cause in the normal one there is no create smh... well it doesn't need it but still!
            repo.CreateNewEntity(sub1);
            repo.CreateNewEntity(sub2);
            repo.CreateNewEntity(sub3); 

            var result = _subcategoryRepository.GetByCategoryId(1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(s => s.ParentCategory.CategoryId == 1));
            CollectionAssert.DoesNotContain(result, sub3);
        }

        [TestMethod]
        public void GetAll_ReturnsAllStoredSubcategories()
        {
            var subcategory1 = new TicketSubcategory(1, "Sub1", 101, _testCategory);
            var subcategory2 = new TicketSubcategory(2, "Sub2", 102, _testCategory);

            var otherCategory = new TicketCategory(2, "FMI", TicketUrgencyLevelEnum.LOW);
            var subcategory3 = new TicketSubcategory(3, "Sub3", 103, otherCategory);

            var repo = (InMemoryTicketSubcategoryRepository)_subcategoryRepository;
            repo.CreateNewEntity(subcategory1);
            repo.CreateNewEntity(subcategory2);
            repo.CreateNewEntity(subcategory3);

            var result = _subcategoryRepository.GetAll().ToList();

            Assert.AreEqual(3, result.Count);
        }
    }
}