using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.dto.mappingProfiles;
using CloudSpritzers1.src.model.faq;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.service.implementation;
using CloudSpritzers1.src.service.interfaces;
using CloudSpritzers1.src.viewModel.faq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;

namespace CloudSpritzers1.src.viewModel.faq
{
    [TestClass]
    public class FAQViewModelTests
    {
        private IFAQRepository _faqRepository;
        private IMapper _mapper;
        private IFAQService _faqService;
        private FAQViewModel _faqViewModel;

        [TestInitialize]
        public void Setup()
        {
            _faqRepository = Substitute.For<IFAQRepository>();
            _mapper = Substitute.For<IMapper>();
            _faqService = Substitute.For<IFAQService>();

            _mapper.Map<FAQEntryDTO>(Arg.Any<FAQEntry>()).Returns(callInfo => MapToDto((FAQEntry)callInfo[0]));
            _mapper.Map<FAQEntry>(Arg.Any<FAQEntryDTO>()).Returns(callInfo => MapToEntity((FAQEntryDTO)callInfo[0]));
            //_mapper.Map<List<FAQEntryDTO>>(Arg.Any<List<FAQEntry>>()).Returns(callInfo =>
            //    new List<FAQEntryDTO>(((List<FAQEntry>)callInfo[0]).Select(e => MapToDto(e))));

            var entries = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
        
            _faqService.GetAll().Returns(entries);
            _faqService.FilterFAQEntry(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(entries);

            _faqViewModel = new FAQViewModel(_faqService, _mapper);
            _faqViewModel.LoadFAQ();
        }

        [TestMethod]
        public void ConstructorLoadsFAQs()
        {
            var allFAQs = _faqService.GetAll();
            _faqService.FilterFAQEntry(Arg.Any<FAQCategoryEnum>(), Arg.Any<string>()).Returns(allFAQs);

            Assert.AreEqual(3, _faqViewModel.FAQs.Count);
            Assert.AreEqual(3, _faqViewModel.FilteredFAQs.Count);
            Assert.AreEqual(FAQCategoryEnum.All, _faqViewModel.SelectedCategory);
            Assert.AreEqual(string.Empty, _faqViewModel.SearchQuery);

            AssertFaqMatches(_faqViewModel.FAQs[0], allFAQs[0]);
            AssertFaqMatches(_faqViewModel.FilteredFAQs[2], allFAQs[2]);
        }

        [TestMethod]
        public void SearchByQuestionOrAnswer()
        {
            var searchResults = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
            };
            _faqService.FilterFAQEntry(FAQCategoryEnum.All, "park").Returns(searchResults);

            _faqViewModel.SearchQuery = "park";

            Assert.AreEqual(2, _faqViewModel.FilteredFAQs.Count);
            CollectionAssert.AreEqual(new[] { 1, 2 }, _faqViewModel.FilteredFAQs.Select(x => x.Id).ToArray());
        }

        [TestMethod]
        public void FilterByCategory()
        {
            var parkingEntries = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
            };
            _faqService.FilterFAQEntry(FAQCategoryEnum.Parking, Arg.Any<string>()).Returns(parkingEntries);
            _faqViewModel.FilterByCategory(FAQCategoryEnum.Parking);

            Assert.AreEqual(FAQCategoryEnum.Parking, _faqViewModel.SelectedCategory);
            Assert.AreEqual(2, _faqViewModel.FilteredFAQs.Count);
            CollectionAssert.AreEqual(new[] { 1, 2 }, _faqViewModel.FilteredFAQs.Select(x => x.Id).ToArray());
        }

        [TestMethod]
        public void AddFAQEntryAsAdminSucceeds()
        {
            _faqViewModel.IsAdmin = true;
            var newEntry = new FAQEntry(4, "How much can the baggage on the plane be?", "10kg", FAQCategoryEnum.Baggage, 0, 0, 0);
            var allFAQs = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
                newEntry
            };

            var newDto = MapToDto(newEntry);

            _faqViewModel.AddFAQEntry(newDto);

            _faqService.Received(1).AddFAQEntry(Arg.Is<FAQEntry>(x => x.Id == newEntry.Id && x.Question == newEntry.Question));
            Assert.AreEqual(3, _faqViewModel.FAQs.Count);
            Assert.AreEqual(3, _faqViewModel.FilteredFAQs.Count);
        }

        [TestMethod]
        public void AddFAQEntryNotAdminThrowsUnauthorizedAccessException()
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => _faqViewModel.AddFAQEntry(MapToDto(new FAQEntry(4, "Q", "A", FAQCategoryEnum.Baggage, 0, 0, 0))));
            _faqService.DidNotReceive().AddFAQEntry(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void EditFAQEntryAsAdminSucceeds()
        {
            _faqViewModel.IsAdmin = true;
            var updatedEntry = new FAQEntry(1, "What cars can I park here?", "Only BMWs", FAQCategoryEnum.Parking, 5, 2, 1);
            var allFAQs = new List<FAQEntry>
            {
                updatedEntry,
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
            _faqService.GetAll().Returns(allFAQs);

            var updatedDto = MapToDto(updatedEntry);

            _faqViewModel.EditFAQEntry(updatedDto);

            _faqService.Received(1).EditFAQEntry(Arg.Is<FAQEntry>(x => x.Id == 1), 1);
            Assert.AreEqual("Only BMWs", _faqViewModel.FAQs.First(x => x.Id == 1).Answer);
        }

        [TestMethod]
        public void EditFAQEntryNotAdminThrowsUnauthorizedAccessException()
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => _faqViewModel.EditFAQEntry(MapToDto(new FAQEntry(4, "Q", "A", FAQCategoryEnum.Baggage, 0, 0, 0))));
            _faqService.DidNotReceive().EditFAQEntry(Arg.Any<FAQEntry>(), Arg.Any<int>());
        }

        [TestMethod]
        public void EditFAQEntryThrowsArgumentNullException()
        {
            _faqViewModel.IsAdmin = true;
            Assert.ThrowsExactly<ArgumentNullException>(() => _faqViewModel.EditFAQEntry(null));
            _faqService.DidNotReceive().EditFAQEntry(Arg.Any<FAQEntry>(), Arg.Any<int>());
        }

        [TestMethod]
        public void DeleteFAQEntryAsAdminSucceeds()
        {
            _faqViewModel.IsAdmin = true;
            var entryToDelete = new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0);
            var updatedEntries = new List<FAQEntry>
            {
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };
            _faqService.GetAll().Returns(updatedEntries);

            var entryToDeleteDto = MapToDto(entryToDelete);

            _faqViewModel.DeleteFAQEntry(entryToDeleteDto);

            _faqService.Received(1).DeleteFAQEntry(entryToDelete.Id);
            Assert.AreEqual(2, _faqViewModel.FAQs.Count);
        }

        [TestMethod]
        public void DeleteFAQEntryNotAdminThrowsUnauthorizedAccessException()
        {
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => _faqViewModel.DeleteFAQEntry(MapToDto(new FAQEntry(4, "Q", "A", FAQCategoryEnum.Baggage, 0, 0, 0))));
            _faqService.DidNotReceive().DeleteFAQEntry(Arg.Any<int>());
        }

        [TestMethod]
        public void DeleteFAQEntryThrowsArgumentNullException()
        {
            _faqViewModel.IsAdmin = true;
            Assert.ThrowsExactly<ArgumentNullException>(() => _faqViewModel.DeleteFAQEntry(null));
            _faqService.DidNotReceive().DeleteFAQEntry(Arg.Any<int>());
        }

        [TestMethod]
        public void ToggleFAQExpandsEntryAndIncrementsViewCount()
        {
            var firstFaq = _faqViewModel.FilteredFAQs[0];
            var secondFaq = _faqViewModel.FilteredFAQs[1];

            _faqViewModel.ToggleFAQ(firstFaq);

            Assert.IsTrue(firstFaq.IsExpanded);
            Assert.IsFalse(secondFaq.IsExpanded);
            Assert.AreEqual(firstFaq, _faqViewModel.SelectedFAQEntry);
            Assert.AreEqual(2, _faqViewModel.FAQs.First(x => x.Id == firstFaq.Id).ViewCount);
            Assert.AreEqual(2, _faqViewModel.FilteredFAQs.First(x => x.Id == firstFaq.Id).ViewCount);
            _faqService.Received(1).IncrementViewCount(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void ToggleFAQCalledForNullEntityReturns()
        {
            var firstFaq = _faqViewModel.FilteredFAQs[0];
            _faqViewModel.ToggleFAQ(null);

            Assert.IsFalse(firstFaq.IsExpanded);
            _faqService.DidNotReceive().IncrementViewCount(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void IncrementWasHelpfulVotes()
        {
            _faqViewModel.SelectedFAQEntry = _faqViewModel.FilteredFAQs[0];

            _faqViewModel.IncrementWasHelpfulVotes();

            _faqService.Received(1).IncrementWasHelpfulVotes(Arg.Any<FAQEntry>());
            Assert.AreEqual(2, _faqViewModel.SelectedFAQEntry!.HelpfulVotesCount);
        }

        [TestMethod]
        public void IncrementHelpfulCount_WithNoSelectedFAQ_DoesNothing()
        {
            _faqViewModel.IncrementWasHelpfulVotes();

            _faqService.DidNotReceive().IncrementWasHelpfulVotes(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void IncrementWasNotHelpfulVotes()
        {
            _faqViewModel.SelectedFAQEntry = _faqViewModel.FilteredFAQs[0];

            _faqViewModel.IncrementWasNotHelpfulVotes();

            _faqService.Received(1).IncrementWasNotHelpfulVotes(Arg.Any<FAQEntry>());
            Assert.AreEqual(1, _faqViewModel.SelectedFAQEntry!.NotHelpfulVotesCount);
        }

        [TestMethod]
        public void IncrementNotHelpfulCount_WithNoSelectedFAQ_DoesNothing()
        {
            _faqViewModel.IncrementWasNotHelpfulVotes();

            _faqService.DidNotReceive().IncrementWasNotHelpfulVotes(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void IncrementViewCount_WithNoSelectedFAQ_DoesNothing()
        {
            _faqViewModel.IncrementViewCount();

            _faqService.DidNotReceive().IncrementViewCount(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void IncrementViewCount_WithValidFAQ_IsSuccessful()
        {
            _faqViewModel.SelectedFAQEntry = _faqViewModel.FAQs[0];
            var faqEntryToIncrementViewCount = _faqViewModel.SelectedFAQEntry;
            var updatedEntries = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 2, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "Parking is 100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
                new FAQEntry(3, "Can I bring my dog on the plane?", "Only if you buy a ticket for him also", FAQCategoryEnum.Baggage, 3, 4, 2),
            };

            _faqService.GetAll().Returns(updatedEntries);

            _faqViewModel.IncrementViewCount();
            var updatedFaqEntry = _faqViewModel.FAQs[0];

            _faqService.Received(1).IncrementViewCount(MapToEntity(faqEntryToIncrementViewCount));
            Assert.AreEqual(updatedFaqEntry.ViewCount, faqEntryToIncrementViewCount.ViewCount + 1);
        }

        [TestMethod]
        public async Task Save_WithNewEntry_AddsFaq()
        {
            _faqViewModel.IsAdmin = true;

            await _faqViewModel.Save("Can my dog come on the plane?", "Depending on the breed", FAQCategoryEnum.Baggage.ToString());

            _faqService.Received(1).AddFAQEntry(Arg.Is<FAQEntry>(x =>
                x.Id == 0 &&
                x.Question == "Can my dog come on the plane?" &&
                x.Answer == "Depending on the breed" &&
                x.Category == FAQCategoryEnum.Baggage));
        }

        [TestMethod]
        public async Task Save_WithExistingEntry_EditsFaq()
        {
            _faqViewModel.IsAdmin = true;
            _faqViewModel.SelectedFAQEntry = _faqViewModel.FAQs[0];

            await _faqViewModel.Save("Can my dog come on the plane?", "Depending on the size", FAQCategoryEnum.Parking.ToString());

            _faqService.Received(1).EditFAQEntry(
                Arg.Is<FAQEntry>(x =>
                    x.Id == _faqViewModel.FAQs[0].Id &&
                    x.Question == "Can my dog come on the plane?" &&
                    x.Answer == "Depending on the size" &&
                    x.Category == FAQCategoryEnum.Parking),
                _faqViewModel.FAQs[0].Id);
        }

        [TestMethod]
        public async Task Save_WithEmptyQuestion_ThrowsArgumentException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await _faqViewModel.Save("   ", "Depending on the size", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithEmptyAnswer_ThrowsArgumentException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await _faqViewModel.Save("Can my dog come on the plane?", "   ", FAQCategoryEnum.Parking.ToString()));
        }

        [TestMethod]
        public async Task Save_WithInvalidCategory_ThrowsArgumentException()
        {
            await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
                await _faqViewModel.Save("Can my dog come on the plane?", "Depending on the size", "NotARealCategory"));
        }

        [TestMethod]
        public void SetCategory_UpdatesCategoryAndFilters()
        {
            var parkingEntries = new List<FAQEntry>
            {
                new FAQEntry(1, "What cars can I park here?", "Only Audis", FAQCategoryEnum.Parking, 1, 1, 0),
                new FAQEntry(2, "How much does parking cost per day?", "100 euros", FAQCategoryEnum.Parking, 2, 3, 1),
            };
            _faqService.FilterFAQEntry(FAQCategoryEnum.Parking, Arg.Any<string>()).Returns(parkingEntries);

            _faqViewModel.SetCategory(FAQCategoryEnum.Parking);

            Assert.AreEqual(FAQCategoryEnum.Parking, _faqViewModel.SelectedCategory);
            Assert.AreEqual(2, _faqViewModel.FilteredFAQs.Count);
        }

        [TestMethod]
        public void GiveFeedback_Helpful_UpdatesFlagsAndVotes()
        {
            var faq = _faqViewModel.FilteredFAQs[0];
            var initialHelpfulVotes = faq.HelpfulVotesCount;

            _faqViewModel.GiveFeedback(faq, true);

            _faqService.Received(1).IncrementWasHelpfulVotes(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialHelpfulVotes + 1, faq.HelpfulVotesCount);
            Assert.IsTrue(faq.HasFeedback);
            Assert.IsTrue(faq.IsHelpfulSelected);
            Assert.IsFalse(faq.IsNotHelpfulSelected);
            Assert.AreEqual(faq, _faqViewModel.SelectedFAQEntry);
        }

        [TestMethod]
        public void GiveFeedback_NotHelpful_UpdatesFlagsAndVotes()
        {
            var faq = _faqViewModel.FilteredFAQs[0];
            var initialNotHelpfulVotes = faq.NotHelpfulVotesCount;

            _faqViewModel.GiveFeedback(faq, false);

            _faqService.Received(1).IncrementWasNotHelpfulVotes(Arg.Any<FAQEntry>());
            Assert.AreEqual(initialNotHelpfulVotes + 1, faq.NotHelpfulVotesCount);
            Assert.IsTrue(faq.HasFeedback);
            Assert.IsFalse(faq.IsHelpfulSelected);
            Assert.IsTrue(faq.IsNotHelpfulSelected);
            Assert.AreEqual(faq, _faqViewModel.SelectedFAQEntry);
        }

        [TestMethod]
        public void GiveFeedback_WithNullFaq_DoesNothing()
        {
            _faqViewModel.GiveFeedback(null, true);

            _faqService.DidNotReceive().IncrementWasHelpfulVotes(Arg.Any<FAQEntry>());
            _faqService.DidNotReceive().IncrementWasNotHelpfulVotes(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void BuildNavigationData_ReturnsExpectedValues()
        {
            _faqViewModel.IsAdmin = true;
            _faqViewModel.SelectedFAQEntry = _faqViewModel.FAQs[1];

            var result = _faqViewModel.BuildNavigationData(42);

            Assert.AreEqual(42, result.CurrentPersonId);
            Assert.IsTrue(result.IsEmployee);
            Assert.AreEqual(_faqViewModel.SelectedFAQEntry, result.FAQEntry);
        }

        private static FAQEntryDTO MapToDto(FAQEntry entry)
        {
            return new FAQEntryDTO(
                entry.Id,
                entry.Question,
                entry.Answer,
                entry.Category,
                entry.ViewCount,
                entry.HelpfulVotesCount,
                entry.NotHelpfulVotesCount);
        }

        private static FAQEntry MapToEntity(FAQEntryDTO dto)
        {
            return new FAQEntry(
                dto.Id,
                dto.Question,
                dto.Answer,
                dto.Category,
                dto.ViewCount,
                dto.HelpfulVotesCount,
                dto.NotHelpfulVotesCount);
        }

        private static void AssertFaqMatches(FAQEntryDTO actual, FAQEntry expected)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Question, actual.Question);
            Assert.AreEqual(expected.Answer, actual.Answer);
            Assert.AreEqual(expected.Category, actual.Category);
            Assert.AreEqual(expected.ViewCount, actual.ViewCount);
            Assert.AreEqual(expected.HelpfulVotesCount, actual.HelpfulVotesCount);
            Assert.AreEqual(expected.NotHelpfulVotesCount, actual.NotHelpfulVotesCount);
        }

        [TestMethod]
        public void IncrementViewCountFor_FaqNotFound_ReturnsWithoutCallingService()
        {
            var nonExistingId = 999;

            _faqViewModel.IncrementViewCountFor(nonExistingId);

            _faqService.DidNotReceive().IncrementViewCount(Arg.Any<FAQEntry>());
        }

        [TestMethod]
        public void IncrementViewCountFor_FilteredFaqSameInstance_DoesNotDuplicateUpdate()
        {
            var faq = _faqViewModel.FAQs[0];

            _faqViewModel.FilteredFAQs.Clear();
            _faqViewModel.FilteredFAQs.Add(faq);

            _faqViewModel.IncrementViewCountFor(faq.Id);

            _faqService.Received(1).IncrementViewCount(Arg.Any<FAQEntry>());
            Assert.AreEqual(2, faq.ViewCount);
        }

        [TestMethod]
        public void ToggleFAQ_WhenCollapsing_SetsSelectedToNull()
        {
            var faq = _faqViewModel.FilteredFAQs[0];

            _faqViewModel.ToggleFAQ(faq);
            Assert.IsTrue(faq.IsExpanded);

            _faqViewModel.ToggleFAQ(faq);

            Assert.IsFalse(faq.IsExpanded);
            Assert.IsNull(_faqViewModel.SelectedFAQEntry);
        }

        [TestMethod]
        public void OnPropertyChanged_RaisesEvent()
        {
            bool eventRaised = false;

            _faqViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_faqViewModel.FAQs))
                    eventRaised = true;
            };

            _faqViewModel.FAQs = new System.Collections.ObjectModel.ObservableCollection<FAQEntryDTO>();

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void IncrementViewCountFor_RaisesPropertyChanged()
        {
            var faq = _faqViewModel.FAQs[0];
            bool faqsChanged = false;
            bool filteredChanged = false;

            _faqViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_faqViewModel.FAQs))
                    faqsChanged = true;
                if (e.PropertyName == nameof(_faqViewModel.FilteredFAQs))
                    filteredChanged = true;
            };

            _faqViewModel.IncrementViewCountFor(faq.Id);

            Assert.IsTrue(faqsChanged);
            Assert.IsTrue(filteredChanged);
        }

        [TestMethod]
        public void IncrementViewCountFor_FilteredFaqDifferentInstance_UpdatesBoth()
        {
            var faq = _faqViewModel.FAQs[0];

            var separateInstance = new FAQEntryDTO(
                faq.Id,
                faq.Question,
                faq.Answer,
                faq.Category,
                faq.ViewCount,
                faq.HelpfulVotesCount,
                faq.NotHelpfulVotesCount
            );

            _faqViewModel.FilteredFAQs.Clear();
            _faqViewModel.FilteredFAQs.Add(separateInstance);

            _faqViewModel.IncrementViewCountFor(faq.Id);

            _faqService.Received(1).IncrementViewCount(Arg.Any<FAQEntry>());

            Assert.AreEqual(faq.ViewCount, separateInstance.ViewCount);
        }

        [TestMethod]
        public void FilteredFAQs_Setter_UpdatesValue_AndRaisesPropertyChanged()
        {
            bool eventRaised = false;

            _faqViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_faqViewModel.FilteredFAQs))
                    eventRaised = true;
            };

            var newCollection = new System.Collections.ObjectModel.ObservableCollection<FAQEntryDTO>();

            _faqViewModel.FilteredFAQs = newCollection;

            Assert.AreEqual(newCollection, _faqViewModel.FilteredFAQs);
            Assert.IsTrue(eventRaised);
        }
    }
}
