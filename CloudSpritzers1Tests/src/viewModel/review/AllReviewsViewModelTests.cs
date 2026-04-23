using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.View.Faq;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using AutoMapper;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.ViewModel.Review;
namespace CloudSpritzers1Tests.Src.ViewModel
{
    [TestClass]
    public class AllReviewsViewModelTests
    {
        private AllReviewsViewModel _viewModel;
        private IRepository<int, Review> _mockRepo;
        private IMapper _mapper;
        private User _testUser;

        [TestInitialize]
        public void Setup()
        {
            // 1. Setup real AutoMapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
            _mapper = config.CreateMapper();

            // 2. Setup Service with Mock Repo
            _mockRepo = Substitute.For<IRepository<int, Review>>();
            var service = new ReviewService(_mockRepo);

            _testUser = new User(1, "Test", "test@test.com");

            _viewModel = new AllReviewsViewModel(service, _mapper);
        }

        [TestMethod]
        public void LoadData_WhenNoReviewsExist_DoesNotCalculateAverages()
        {
           
            _mockRepo.GetAll().Returns(new List<Review>());

            _viewModel.LoadData();

            Assert.AreEqual(0, _viewModel.Reviews.Count);
            Assert.AreEqual(0, _viewModel.TotalReviews);
        }

        [TestMethod]
        public void LoadData_WhenReviewsExist_CalculatesAveragesAndFormatsStrings()
        {
            
            var reviews = new List<Review>
    {
        new Review(1, _testUser, "Good", 5, 4, 3, 2),
        new Review(2, _testUser, "Bad", 1, 2, 3, 4)
    };
            _mockRepo.GetAll().Returns(reviews);

           
            _viewModel.LoadData();

            
            // Average of 5 and 1 is 3.0
            Assert.AreEqual(3.0, _viewModel.AverageDutyFree);
            Assert.AreEqual("3.0", _viewModel.FormattedAverageDutyFree);

            // Average of 4 and 2 is 3.0
            Assert.AreEqual("3.0", _viewModel.FormattedAverageFlightExperience);

            Assert.AreEqual(2, _viewModel.TotalReviews);
        }

        [TestMethod]
        public void FormattedProperties_ReturnCorrectOneDecimalString()
        {
           
            var reviews = new List<Review>
    {
        new Review(1, _testUser, "Test", 1, 1, 4, 2), // Staff: 4, Clean: 2
        new Review(2, _testUser, "Test", 1, 1, 5, 3)  // Staff: 5, Clean: 3
    };
            _mockRepo.GetAll().Returns(reviews);

           
            _viewModel.LoadData();

            // Average Staff: 4.5
            Assert.AreEqual("4.5", _viewModel.FormattedAverageStaffFriendliness);
            // Average Clean: 2.5
            Assert.AreEqual("2.5", _viewModel.FormattedAverageCleanliness);
        }

        [TestMethod]
        public void LoadData_WhenReviewsExist_PopulatesCollectionWithMappedDTOs()
        {
          
            var reviews = new List<Review>
    {
        new Review(1, _testUser, "Excellent service", 5, 5, 5, 5)
    };
            _mockRepo.GetAll().Returns(reviews);

            _viewModel.LoadData();

            Assert.AreEqual(1, _viewModel.Reviews.Count);
            Assert.AreEqual("Excellent service", _viewModel.Reviews[0].message);
           
            Assert.IsInstanceOfType(_viewModel.Reviews[0], typeof(ReviewDTO));
        }

        [TestMethod]
        public void LoadData_WhenServiceReturnsNull_ReturnsEarlyWithoutError()
        {
            _mockRepo.GetAll().Returns((IEnumerable<Review>)null!);

            _viewModel.LoadData();

            Assert.AreEqual(0, _viewModel.Reviews.Count, "Collection should be empty if service returns null");
        }

    }
}