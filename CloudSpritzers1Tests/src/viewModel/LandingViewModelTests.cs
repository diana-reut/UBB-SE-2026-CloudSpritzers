using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CloudSpritzers1Tests.Src.ViewModel
{
    [TestClass]
    public class LandingViewModelTests
    {
        private IRepository<int, Review> _reviewRepository;
        private ReviewService _reviewService;
        private IMapper _mapper;
        private LandingViewModel _landingViewModel;

        private User _testUser;
        private Review _testReview;

        [TestInitialize]
        public void Setup()
        {

            _reviewRepository = Substitute.For<IRepository<int, Review>>();
            _mapper = Substitute.For<IMapper>();

            _reviewService = new ReviewService(_reviewRepository);

            _testUser = new User(1, "Alexandru Popescu", "alex.p@email.com");
            _testReview = new Review(10, _testUser, "Great airport!", 5, 4, 5, 5);

            _reviewRepository.GetAll().Returns(new List<Review> { _testReview });
            _mapper.Map<ReviewDTO>(Arg.Any<Review>()).Returns(callInfo => MapToDto((Review)callInfo[0]));


            _landingViewModel = new LandingViewModel(_reviewService, _mapper);
        }

        [TestMethod]
        public void Constructor_InitializesAndLoadsReviews()
        {
            Assert.AreEqual(1, _landingViewModel.Reviews.Count);

            var reviewResult = _landingViewModel.Reviews[0];
            Assert.AreEqual("Alexandru Popescu", reviewResult.userName);

            Assert.AreEqual(4.75f, reviewResult.overallRating);
        }

        [TestMethod]
        public void LoadReviews_ClearsExistingCollectionBeforeAdding()
        {
            _landingViewModel.LoadReviews();

            Assert.AreEqual(1, _landingViewModel.Reviews.Count);
        }

        private static ReviewDTO MapToDto(Review review)
        {
            return new ReviewDTO(
                review.GetId(),
                review.GetUser().UserId,
                "",
                review.GetMessage(),
                review.GetDutyFreeRating(),
                review.GetFlightExperienceRating(),
                review.GetStaffFriendlinessRating(),
                review.GetCleanlinessRating(),
                0f
            );
        }
    }
}