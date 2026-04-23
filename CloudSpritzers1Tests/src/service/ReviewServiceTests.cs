using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Model;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.Src.Repository;

namespace CloudSpritzers1Tests.Src.Service
{
    [TestClass]
    public class ReviewServiceTests
    {
        private IRepository<int, Review> _reviewRepository;
        private ReviewService _reviewService;
        private User _testUser;

        [TestInitialize]
        public void Setup()
        {
            // 1. Mock the repository using NSubstitute
            _reviewRepository = Substitute.For<IRepository<int, Review>>();

            // 2. Inject the mock into the service
            _reviewService = new ReviewService(_reviewRepository);

            // 3. Create a real User for test data
            _testUser = new User(1, "Test User", "test@test.com");

            // 4. Setup a default "GetAll" return to prevent null reference errors during validation
            _reviewRepository.GetAll().Returns(new List<Review>());
        }

        [TestMethod]
        public void CalculateAverageRating_WhenCalled_ReturnsCorrectMath()
        {
            var review = new Review(1, _testUser, "Test", 1, 2, 3, 4); // Sum = 10
           
            var result = _reviewService.CalculateAverageRating(review);

            Assert.AreEqual(2.5f, result);
        }

        [TestMethod]
        public void CreateReview_WithValidData_CallsRepositoryToSave()
        {
        
            _reviewService.CreateReview(1, _testUser, "Great flight", 5, 5, 5, 5);

            _reviewRepository.Received(1).CreateNewEntity(Arg.Any<Review>());
        }

        [TestMethod]
        public void ValidateReview_RatingBelowMin_ThrowsArgumentException()
        {
            // Arrange: Set DutyFree to 0 (Min is 1)
            var review = new Review(1, _testUser, "Too low", 0, 5, 5, 5);

           
            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Duty Free Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_RatingAboveMax_ThrowsArgumentException()
        {
            // Arrange: Set Cleanliness to 6 (Max is 5)
            var review = new Review(1, _testUser, "Too high", 5, 5, 5, 6);

            // Act & Assert
            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_EmptyMessage_ThrowsArgumentException()
        {
            // Arrange
            var review = new Review(1, _testUser, "", 5, 5, 5, 5);

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Message cannot be null or empty", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_NullUser_ThrowsArgumentException()
        {
            // Arrange: Pass null for the user
            var review = new Review(1, null, "No user", 5, 5, 5, 5);

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("User cannot be null", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_DuplicateReview_ThrowsArgumentException()
        {
            var existingReview = new Review(1, _testUser, "I already exist", 5, 5, 5, 5);

            _reviewRepository.GetAll().Returns(new List<Review> { existingReview });

            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(existingReview));

            StringAssert.Contains("Review already exists", ex.Message);
        }

        [TestMethod]
        public void GetById_ValidId_ReturnsReviewFromRepository()
        {
            // Arrange
            var expectedReview = new Review(1, _testUser, "Great", 5, 5, 5, 5);
            _reviewRepository.GetById(1).Returns(expectedReview);

            // Act
            var result = _reviewService.GetById(1);

            // Assert
            Assert.AreEqual(expectedReview, result);
            _reviewRepository.Received(1).GetById(1); 
        }

        [TestMethod]
        public void UpdateById_WhenCalled_CallsRepositoryUpdate()
        {
           
            var updatedReview = new Review(1, _testUser, "Updated Message", 4, 4, 4, 4);
            _reviewService.UpdateById(1, updatedReview);
            _reviewRepository.Received(1).UpdateById(1, updatedReview);
        }

        [TestMethod]
        public void DeleteById_WhenCalled_CallsRepositoryDelete()
        {
            _reviewService.DeleteById(10);
            _reviewRepository.Received(1).DeleteById(10);
        }

        [TestMethod]
        public void GetAll_WhenCalled_ReturnsListOfReviews()
        {
            // Arrange
            var reviews = new List<Review>
        {
        new Review(1, _testUser, "R1", 5, 5, 5, 5),
        new Review(2, _testUser, "R2", 4, 4, 4, 4)
        };
            _reviewRepository.GetAll().Returns(reviews);

            // Act
            var result = _reviewService.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count);
            _reviewRepository.Received(1).GetAll();
        }

        [TestMethod]
        public void ValidateReview_FlightExperienceRatingInvalid_ThrowsArgumentException()
        {
            // Arrange: 0 is below MinRating (1)
            var review = new Review(1, _testUser, "Test", 5, 0, 5, 5);

            // Act & Assert
            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Flight Experience Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_StaffFriendlinessRatingInvalid_ThrowsArgumentException()
        {
            // Arrange: 6 is above MaxRating (5)
            var review = new Review(1, _testUser, "Test", 5, 5, 6, 5);

            // Act & Assert
            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Staff Friendliness Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_CleanlinessRatingInvalid_ThrowsArgumentException()
        {
            // Arrange: 0 is below MinRating (1)
            var review = new Review(1, _testUser, "Test", 5, 5, 5, 0);

            // Act & Assert
            var ex = Assert.ThrowsExactly<ArgumentException>(() =>
                _reviewService.ValidateReview(review));

            StringAssert.Contains("Cleanliness Rating must be between 1 and 5", ex.Message);
        }

        [TestMethod]
        public void ValidateReview_WithAllValidData_DoesNotThrowAndMovesToNextStep()
        {
            // Arrange: All ratings are exactly within the 1-5 range
            var validReview = new Review(1, _testUser, "Everything was perfect!", 5, 5, 5, 5);

            _reviewService.ValidateReview(validReview);
        }

        [TestMethod]
        public void ValidateReview_StaffFriendlinessBelowMin_ThrowsArgumentException()
        {
            // Arrange: 0 is < MinRating (5)
            var review = new Review(1, _testUser, "Test", 5, 5, 0, 5);

            Assert.ThrowsExactly<ArgumentException>(() => _reviewService.ValidateReview(review));
        }






    }
}