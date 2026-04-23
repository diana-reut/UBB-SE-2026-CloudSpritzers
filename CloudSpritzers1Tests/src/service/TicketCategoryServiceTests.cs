using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Ticket;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class TicketCategoryServiceTests
    {
        private ITicketCategoryRepository _categoryRepoMock;
        private TicketCategoryService _categoryService;

        [TestInitialize]
        public void Setup()
        {
            
            _categoryRepoMock = Substitute.For<ITicketCategoryRepository>();
            _categoryService = new TicketCategoryService(_categoryRepoMock);
        }

        [TestMethod]
        public void GetCategoryById_WhenCalled_ReturnsCategoryFromRepository()
        {
           
            var expectedCategory = new TicketCategory(1, "Technical", TicketUrgencyLevelEnum.HIGH);
            _categoryRepoMock.GetById(1).Returns(expectedCategory);          
            var result = _categoryService.GetCategoryById(1);

            Assert.AreEqual(expectedCategory.CategoryName, result.CategoryName);
            _categoryRepoMock.Received(1).GetById(1);
        }

        [TestMethod]
        public void GetAllCategories_WhenCalled_ReturnsAllCategoriesFromRepository()
        {

            var categories = new List<TicketCategory>
            {
                new TicketCategory(1, "IT", TicketUrgencyLevelEnum.MEDIUM),
                new TicketCategory(2, "HR", TicketUrgencyLevelEnum.LOW)
            };
            _categoryRepoMock.GetAll().Returns(categories);

            var result = _categoryService.GetAllCategories().ToList();

            Assert.AreEqual(2, result.Count);
            _categoryRepoMock.Received(1).GetAll();
        }

        [TestMethod]
        public void GetCategoryById_WhenRepositoryThrows_ServicePropagatesException()
        {

            _categoryRepoMock.GetById(Arg.Any<int>()).Returns(x => { throw new KeyNotFoundException(); });

            Assert.ThrowsExactly<KeyNotFoundException>(() => _categoryService.GetCategoryById(999));
        }
    }
}