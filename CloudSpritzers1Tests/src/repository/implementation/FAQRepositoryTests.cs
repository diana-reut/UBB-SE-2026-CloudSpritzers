using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1Tests.Src.MockClasses;

namespace CloudSpritzers1Tests.Src.Repository.Implementation
{
    [TestClass()]
    public class FAQRepositoryTests
    {
        private IFAQRepository faqRepository;

        //[AssemblyInitialize]
        //public static void AssemblyInit(TestContext _)
        //{
        //    // Option A: Load from absolute .env.test path
        //    var baseDir = AppContext.BaseDirectory;
        //    var envPath = Path.GetFullPath(Path.Combine(baseDir, "../../../../src/.env.test"));
        //    DotNetEnv.Env.Load(envPath);

        //    // Option B (most robust): set explicitly (can be used instead of Env.Load)
        //    //Environment.SetEnvironmentVariable("DB_SERVER", "DESKTOP-7CMFPVG\\SQLEXPRESS");
        //    //Environment.SetEnvironmentVariable("DB_NAME", "CloudSpritzersTest");
        //    //Environment.SetEnvironmentVariable("DB_USER", "Ama");
        //    //Environment.SetEnvironmentVariable("DB_PASS", "amalia");
        //}


        [TestInitialize]
        public void Setup()
        {
            
            faqRepository = new InMemoryFAQRepository();
        }

        [TestMethod()]
        public void GetById_WithExistingId_Succeeds()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            faqRepository.CreateNewEntity(entry);
            var expected = entry;

            var result = faqRepository.GetById(1);
            Assert.AreEqual(expected, result);

        }

        [TestMethod()]
        public void GetById_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => faqRepository.GetById(1));
        }

        [TestMethod()]
        public void AddFaq_WithValidEntity_Succeeds()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);

            var resultId = faqRepository.CreateNewEntity(entry);
            Assert.AreEqual(entry.Id, resultId);
        }

        [TestMethod()]
        public void AddFaq_WithDuplicateEntity_ThrowsArgumentException()
        {
            var firstEntry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            var secondEntry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);

            faqRepository.CreateNewEntity(firstEntry);

            Assert.ThrowsExactly<ArgumentException>(() => faqRepository.CreateNewEntity(secondEntry));
        }

        [TestMethod()]
        public void UpdateById_WithExistingId_Succeeds()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            faqRepository.CreateNewEntity(entry);

            var updatedEntry = new FAQEntry(entry.Id, entry.Question, "Only BMWs", FAQCategoryEnum.Parking, 3, entry.HelpfulVotesCount, 6);
            faqRepository.UpdateById(entry.Id, updatedEntry);

            var updateResultEntry = faqRepository.GetById(entry.Id);
            Assert.AreEqual(updatedEntry, updateResultEntry);
        }

        [TestMethod()]
        public void UpdateFaq_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            var firstEntry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            var updatedEntry = new FAQEntry(2, "What cars can I park here?", "Only BMWS", FAQCategoryEnum.Parking, 1, 1, 0);

            faqRepository.CreateNewEntity(firstEntry);

            Assert.ThrowsExactly<KeyNotFoundException>(() => faqRepository.UpdateById(updatedEntry.Id, updatedEntry));
        }

        [TestMethod()]
        public void DeleteById_WithExistingId_Succeeds()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            faqRepository.CreateNewEntity(entry);
  
            faqRepository.DeleteById(1);

            var newEntries = faqRepository.GetAll().ToList();
            var expected = new List<FAQEntry>();
            CollectionAssert.AreEqual(newEntries, expected);
        }

        [TestMethod()]
        public void DeleteFaq_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            var firstEntry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            faqRepository.CreateNewEntity(firstEntry);
            faqRepository.DeleteById(firstEntry.Id);

            Assert.ThrowsExactly<KeyNotFoundException>(() => faqRepository.DeleteById(firstEntry.Id));
        }

        [TestMethod()]
        public void GetAll_ReturnsAllEntities()
        {
            var expected = new List<FAQEntry>();
            var result = faqRepository.GetAll().ToList();
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetByCategory_WithCategoryParking_ReturnsCorrectEntities()
        {
            var expected= new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "100 euros", FAQCategoryEnum.Parking, 200, 3, 1),
            };
            foreach (var e in expected)
                faqRepository.CreateNewEntity(e);

            var result = faqRepository.GetByCategory(FAQCategoryEnum.Parking);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetByCategory_WithCategoryAll_ReturnsAllEntities()
        {
            var expected = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "100 euros", FAQCategoryEnum.Parking, 200, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a plane ticket for him also", FAQCategoryEnum.Baggage, 123, 34, 2),

            };
            foreach (var e in expected)
                faqRepository.CreateNewEntity(e);

            var result = faqRepository.GetByCategory(FAQCategoryEnum.All);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IncrementViewCount_WithExistingId_Succeeds()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            faqRepository.CreateNewEntity(entry);
            var expected = new FAQEntry(entry.Id, entry.Question, entry.Answer, entry.Category, entry.ViewCount + 1, entry.HelpfulVotesCount, entry.NotHelpfulVotesCount);

            faqRepository.IncrementViewCount(entry.Id);
            
            var result = faqRepository.GetById(expected.Id);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IncrementWasHelpfulVotes_WithExistingId_Succeeds()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            faqRepository.CreateNewEntity(entry);
            var expected = new FAQEntry(entry.Id, entry.Question, entry.Answer, entry.Category, entry.ViewCount, entry.HelpfulVotesCount + 1, entry.NotHelpfulVotesCount);

            faqRepository.IncrementWasHelpfulVotes(entry.Id);
            
            var result = faqRepository.GetById(expected.Id);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IncrementWasNotHelpfulVotes_WithExistingId_Succeeds()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            faqRepository.CreateNewEntity(entry);
            var expected = new FAQEntry(entry.Id, entry.Question, entry.Answer, entry.Category, entry.ViewCount, entry.HelpfulVotesCount, entry.NotHelpfulVotesCount + 1);

            faqRepository.IncrementWasNotHelpfulVotes(entry.Id);
            
            var result = faqRepository.GetById(expected.Id);
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void IncrementWasNotHelpfulVotes_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            Assert.ThrowsExactly<KeyNotFoundException>(() => faqRepository.IncrementWasNotHelpfulVotes(entry.Id));
        }

        [TestMethod()]
        public void IncrementWasHelpfulVotes_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            Assert.ThrowsExactly<KeyNotFoundException>(() => faqRepository.IncrementWasHelpfulVotes(entry.Id));
        }

        [TestMethod()]
        public void IncrementViewCount_WithoutExistingId_ThrowsKeyNotFoundException()
        {
            var entry = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            Assert.ThrowsExactly<KeyNotFoundException>(() => faqRepository.IncrementViewCount(entry.Id));
        }
    }
}