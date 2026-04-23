using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.src.service.implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.model.faq;
using CloudSpritzers1.src.service.interfaces;
using CloudSpritzers1.src.dto;
namespace CloudSpritzers1.src.service.implementation.Tests
{
    [TestClass()]
    public class FAQServiceTests
    {
        private IFAQRepository _faqRepo;
        private IFAQService _faqService;
        [TestInitialize]
        public void Setup()
        {
            _faqRepo = Substitute.For<IFAQRepository>();
            _faqService = new FAQService(_faqRepo);

            var faqList = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "100 euros", FAQCategoryEnum.Parking, 200, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a plane ticket for him also", FAQCategoryEnum.Baggage, 123, 34, 2),
            };
            _faqRepo.GetAll().Returns(faqList);
        }

        [TestMethod()]
        public void GetAll_ReturnsAllEntities()
        {
            var expected= new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "100 euros", FAQCategoryEnum.Parking, 200, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a plane ticket for him also", FAQCategoryEnum.Baggage, 123, 34, 2),
            };

            var wrongResult = new FAQEntry(2, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);

            var result = _faqService.GetAll();
            CollectionAssert.AreEqual(expected,result);
            Assert.AreNotEqual(expected[0], wrongResult);
        }

        [TestMethod()]
        public void GetByCategory_WithCategoryBaggage_ReturnsAllEntitiesWithCategoryBaggage()
        {
            var categoryToTest = FAQCategoryEnum.Baggage;
            var expected= new List<FAQEntry>
            {
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a plane ticket for him also", FAQCategoryEnum.Baggage, 123, 34, 2),
            };
            _faqRepo.GetByCategory(categoryToTest).Returns(expected);
            var result = _faqService.GetByCategory(categoryToTest);
            Console.WriteLine(result);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void AddFAQEntry_WithValidEntity_ReturnsAddedId()
        {
            var newFAQEntryToAdd = new FAQEntry(4, "How much can the baggage on the plane be?", "10kg", FAQCategoryEnum.Baggage, 231, 48, 23);
            _faqRepo.CreateNewEntity(newFAQEntryToAdd).Returns(4);

            _faqService.AddFAQEntry(newFAQEntryToAdd);

            _faqRepo.Received(1).CreateNewEntity(newFAQEntryToAdd);
        }

        [TestMethod()]
        public void EditFAQEntry_CallsRepository()
        {
            var FAQEntryToEdit = new FAQEntry(1, "What cars can I park here?", "Only BMWs", FAQCategoryEnum.Parking, 432, 43, 10);

            var updatedFaqList = new List<FAQEntry>
            {
                FAQEntryToEdit,
                new FAQEntry(2, "How much does parking cost per day?", "100 euros", FAQCategoryEnum.Parking, 200, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a plane ticket for him also", FAQCategoryEnum.Baggage, 123, 34, 2),
            };
            _faqRepo.GetAll().Returns(updatedFaqList);

            _faqService.EditFAQEntry(FAQEntryToEdit, 1);
            var updatedEntity = _faqService.GetAll()[0];

            _faqRepo.Received(1).UpdateById(1, FAQEntryToEdit);
            Assert.AreEqual(updatedEntity, FAQEntryToEdit);
        }

        [TestMethod()]
        public void DeleteFAQEntry_CallsRepository()
        {
            var FAQIdToDelete = 4;
            var newFAQEntryToAdd = new FAQEntry(FAQIdToDelete, "How much can the baggage on the plane be?", "10kg", FAQCategoryEnum.Baggage, 231, 48, 23);
            _faqRepo.CreateNewEntity(newFAQEntryToAdd).Returns(FAQIdToDelete);

            _faqService.DeleteFAQEntry(FAQIdToDelete);

            _faqRepo.Received(1).DeleteById(FAQIdToDelete);
        }

        [TestMethod()]
        public void IncrementViewCount_CallsRepository()
        {
            var FAQEntryToIncrementViewCount = _faqRepo.GetAll().First();

            _faqService.IncrementViewCount(FAQEntryToIncrementViewCount);

            _faqRepo.Received(1).IncrementViewCount(FAQEntryToIncrementViewCount.Id);
        }

        [TestMethod()]
        public void IncrementWasHelpfulVotes_CallsRepository()
        {
            var FAQEntryToIncrementHelpfulVotesCount = _faqRepo.GetAll().First();

            _faqService.IncrementWasHelpfulVotes(FAQEntryToIncrementHelpfulVotesCount);

            _faqRepo.Received(1).IncrementWasHelpfulVotes(FAQEntryToIncrementHelpfulVotesCount.Id);
        }

        [TestMethod()]
        public void IncrementWasNotHelpfulVotes_CallsRepository()
        {
            var FAQEntryToIncrementNotHelpfulVotesCount = _faqRepo.GetAll().First();

            _faqService.IncrementWasNotHelpfulVotes(FAQEntryToIncrementNotHelpfulVotesCount);

            _faqRepo.Received(1).IncrementWasNotHelpfulVotes(FAQEntryToIncrementNotHelpfulVotesCount.Id);
        }

        [TestMethod()]
        public void FilterFAQEntry_WithCategoryAndSearch_ReturnsFilteredEntities()
        {
            var FAQCatgoryToFilterBy = FAQCategoryEnum.Parking;
            var SearchQueryToFilterBy = "cars";

            var expected = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
            };

            _faqRepo.GetByCategory(FAQCategoryEnum.Parking).Returns(expected);
            
            var result = _faqService.FilterFAQEntry(FAQCatgoryToFilterBy, SearchQueryToFilterBy);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void SearchMatchEntry_WithNoMatchingString_ReturnsEmptyList()
        {
            var FAQCatgoryToFilterBy = FAQCategoryEnum.All;
            var SearchQueryToFilterBy = "water";

            var expected = new List<FAQEntry>();

            _faqRepo.GetByCategory(FAQCatgoryToFilterBy).Returns(expected);

            var result = _faqService.FilterFAQEntry(FAQCatgoryToFilterBy, SearchQueryToFilterBy);
            Assert.AreEqual(0, result.Count());
            CollectionAssert.AreEqual(expected, result);
        }
    }
}