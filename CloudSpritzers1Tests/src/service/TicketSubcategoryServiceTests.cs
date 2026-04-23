using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class TicketSubcategoryServiceTests
    {
        private ITicketSubcategoryRepository _subcategoryRepoMock;
        private TicketSubcategoryService _subcategoryService;
        private TicketCategory _testCategory;

        [TestInitialize]
        public void Setup()
        {
            _subcategoryRepoMock = Substitute.For<ITicketSubcategoryRepository>();
            _subcategoryService = new TicketSubcategoryService(_subcategoryRepoMock);

            _testCategory = new TicketCategory(1, "IT", TicketUrgencyLevelEnum.HIGH);
        }

        [TestMethod]
        public void GetSubcategoryById_WhenCalled_ReturnsEntityFromRepository()
        {
            var expectedSub = new TicketSubcategory(10, "Software", 500, _testCategory);
            _subcategoryRepoMock.GetById(10).Returns(expectedSub);

            var result = _subcategoryService.GetSubcategoryById(10);

            Assert.AreEqual("Software", result.SubcategoryName);
            _subcategoryRepoMock.Received(1).GetById(10);
        }

        [TestMethod]
        public void GetSubcategoriesByCategoryId_WhenCalled_ReturnsFilteredList()
        {
            var expectedList = new List<TicketSubcategory>
            {
                new TicketSubcategory(10, "Software", 500, _testCategory),
                new TicketSubcategory(11, "Hardware", 501, _testCategory)
            };
            _subcategoryRepoMock.GetByCategoryId(1).Returns(expectedList);

            var result = _subcategoryService.GetSubcategoriesByCategoryId(1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Software", result[0].SubcategoryName);
            _subcategoryRepoMock.Received(1).GetByCategoryId(1);
        }

        [TestMethod]
        public void GetSubcategoryById_WhenIdIsInvalid_PropagatesException()
        {

            _subcategoryRepoMock.GetById(999).Returns(x => { throw new KeyNotFoundException(); });
            Assert.ThrowsExactly<KeyNotFoundException>(() => _subcategoryService.GetSubcategoryById(999));
        }
    }
}