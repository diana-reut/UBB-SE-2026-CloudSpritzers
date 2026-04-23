using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CloudSpritzers1Tests.Src.Dto.MappingProfiles
{
    [TestClass]
    public class ReviewMappingProfileTests
    {
        private IMapper _mapper;

        [TestInitialize]
        public void Setup()
        {
            // Initialize AutoMapper with the specific profile
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ReviewMappingProfile>());
            _mapper = configuration.CreateMapper();
        }

        [TestMethod]
        public void MapFromReviewToReviewDTO_ValidReview_AllFieldsMappedCorrectly()
        {
            var user = new User(101, "John Doe", "john@example.com");
            var sourceReview = new Review(1, user, "Great flight!", 5, 4, 3, 2);

            
            var resultDto = _mapper.Map<ReviewDTO>(sourceReview);

            
            Assert.IsNotNull(resultDto);
            Assert.AreEqual(sourceReview.GetId(), resultDto.reviewId);
            Assert.AreEqual(sourceReview.GetUser().UserId, resultDto.userId);
            Assert.AreEqual("John Doe", resultDto.userName);
            Assert.AreEqual(sourceReview.GetMessage(), resultDto.message);
            Assert.AreEqual(sourceReview.GetDutyFreeRating(), resultDto.dutyFreeRating);
            Assert.AreEqual(sourceReview.GetFlightExperienceRating(), resultDto.flightExperienceRating);
            Assert.AreEqual(sourceReview.GetStaffFriendlinessRating(), resultDto.staffFriendlinessRating);
            Assert.AreEqual(sourceReview.GetCleanlinessRating(), resultDto.cleanlinessRating);
        }

        [TestMethod]
        public void MapFromReviewToReviewDTO_AllRatingsEqual_OverallRatingCalculatedCorrectly()
        {
            
            var user = new User(102, "Jane Doe", "jane@example.com");
            // All 4s should equal exactly 4.0 average
            var sourceReview = new Review(2, user, "Good", 4, 4, 4, 4);

           
            var resultDto = _mapper.Map<ReviewDTO>(sourceReview);

            Assert.AreEqual(4.0f, resultDto.overallRating);
        }

        [TestMethod]
        public void MapFromReviewToReviewDTO_ZeroRatings_OverallRatingIsZero()
        {
            
            var user = new User(103, "Bob", "bob@example.com");
            var sourceReview = new Review(3, user, "Bad", 0, 0, 0, 0);

            var resultDto = _mapper.Map<ReviewDTO>(sourceReview);

            Assert.AreEqual(0.0f, resultDto.overallRating);
        }
    }
}
