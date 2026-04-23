using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1Tests.Src.MockClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CloudSpritzers1Tests.Src.Repository
{
    [TestClass]
    public class DecisionTreeRepositoryTests
    {
        private IRepository<int, FAQNode> _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryDecisionTreeRepository();
        }

        [TestMethod]
        public void Add_ValidFAQNode_ReturnsCorrectId()
        {
            var node = new FAQNode(0, "What is your return policy?", ImmutableArray<FAQOption>.Empty, true);
            int id = _repository.CreateNewEntity(node);
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void Add_NullFAQNode_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _repository.CreateNewEntity(null!));
        }

        [TestMethod]
        public void GetById_ExistingFAQNode_ReturnsCorrectQuestionText()
        {
            var node = new FAQNode(0, "What is your return policy?", ImmutableArray<FAQOption>.Empty, true);
            _repository.CreateNewEntity(node);
            var result = _repository.GetById(1);
            Assert.AreEqual("What is your return policy?", result.questionText);
        }

        [TestMethod]
        public void GetById_ExistingFAQNode_ReturnsCorrectIsFinalAnswer()
        {
            var node = new FAQNode(0, "What is your return policy?", ImmutableArray<FAQOption>.Empty, true);
            _repository.CreateNewEntity(node);
            var result = _repository.GetById(1);
            Assert.IsTrue(result.isFinalAnswer);
        }

        [TestMethod]
        public void GetById_ExistingFAQNode_ReturnsCorrectOptionsLength()
        {
            var options = ImmutableArray.Create(new FAQOption("Details", 2));
            var node = new FAQNode(0, "What is your return policy?", options, true);
            _repository.CreateNewEntity(node);
            var result = _repository.GetById(1);
            Assert.AreEqual(1, result.options.Length);
        }

        [TestMethod]
        public void GetById_ExistingFAQNode_ReturnsCorrectOptionLabel()
        {
            var options = ImmutableArray.Create(new FAQOption("Details", 2));
            var node = new FAQNode(0, "What is your return policy?", options, true);
            _repository.CreateNewEntity(node);
            var result = _repository.GetById(1);
            Assert.AreEqual("Details", result.options[0].label);
        }

        [TestMethod]
        public void GetById_ReturnsCorrectFAQNodeId()
        {
            var node = new FAQNode(10, "Test Node", ImmutableArray<FAQOption>.Empty, false);
            int assignedId = _repository.CreateNewEntity(node);
            var result = _repository.GetById(assignedId);
            Assert.AreEqual(assignedId, result.faqNodeId);
        }

        [TestMethod]
        public void GetById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.GetById(999));
        }

        [TestMethod]
        public void DeleteById_ExistingId_ReducesCollectionCountToZero()
        {
            var node = new FAQNode(0, "Test", ImmutableArray<FAQOption>.Empty, false);
            _repository.CreateNewEntity(node);
            _repository.DeleteById(1);
            Assert.AreEqual(0, _repository.GetAll().Count());
        }

        [TestMethod]
        public void DeleteById_NonExistingId_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.DeleteById(999));
        }

        [TestMethod]
        public void UpdateById_ExistingId_UpdatesQuestionText()
        {
            var oldNode = new FAQNode(0, "Old Question", ImmutableArray<FAQOption>.Empty, false);
            _repository.CreateNewEntity(oldNode);
            var updatedNode = new FAQNode(0, "New Question", ImmutableArray<FAQOption>.Empty, true);
            _repository.UpdateById(1, updatedNode);
            var result = _repository.GetById(1);
            Assert.AreEqual("New Question", result.questionText);
        }

        [TestMethod]
        public void UpdateById_ExistingId_UpdatesIsFinalAnswer()
        {
            var oldNode = new FAQNode(0, "Old Question", ImmutableArray<FAQOption>.Empty, false);
            _repository.CreateNewEntity(oldNode);
            var updatedNode = new FAQNode(0, "New Question", ImmutableArray<FAQOption>.Empty, true);
            _repository.UpdateById(1, updatedNode);
            var result = _repository.GetById(1);
            Assert.IsTrue(result.isFinalAnswer);
        }

        [TestMethod]
        public void UpdateById_ExistingId_UpdatesOptionsLength()
        {
            var oldNode = new FAQNode(0, "Old Question", ImmutableArray<FAQOption>.Empty, false);
            _repository.CreateNewEntity(oldNode);
            var newOptions = ImmutableArray.Create(new FAQOption("New Option", 3));
            var updatedNode = new FAQNode(0, "New Question", newOptions, true);
            _repository.UpdateById(1, updatedNode);
            var result = _repository.GetById(1);
            Assert.AreEqual(1, result.options.Length);
        }

        [TestMethod]
        public void UpdateById_NullFAQNode_ThrowsArgumentNullException()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => _repository.UpdateById(1, null!));
        }

        [TestMethod]
        public void UpdateById_NonExistingId_ThrowsKeyNotFoundException()
        {
            var updatedNode = new FAQNode(999, "No Body", ImmutableArray<FAQOption>.Empty, false);
            Assert.ThrowsExactly<KeyNotFoundException>(() => _repository.UpdateById(999, updatedNode));
        }
    }
}