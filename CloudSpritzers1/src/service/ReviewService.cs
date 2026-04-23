using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.review;
using CloudSpritzers1.src.repository;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CloudSpritzers1.src.service
{
    public class ReviewService
    {
        private readonly IRepository<int, Review> _reviewRepository;

        private const int MinRating = 1;
        private const int MaxRating = 5;
        private const int NumberOfRatings = 4;

        public ReviewService(IRepository<int, Review> reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public Review GetById(int id)
        {
            return _reviewRepository.GetById(id);
        }

        public int Add(Review review)
        {
            return _reviewRepository.CreateNewEntity(review);
        }

        public void UpdateById(int id, Review review)
        {
            _reviewRepository.UpdateById(id, review);
        }

        public void DeleteById(int id)
        {
            _reviewRepository.DeleteById(id);
        }

        public List<Review> GetAll()
        {
            return _reviewRepository.GetAll().ToList();
        }

        public void CreateReview(int id, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating)
        {
            Review review = new(id, user, message, dutyFreeRating, flightExperienceRating, staffFriendlinessRating, cleanlinessRating);
            ValidateReview(review);
            Add(review);
        }

        public void ValidateReview(Review review)
        {
            ArgumentNullException.ThrowIfNull(review);
        
            if (this.GetAll().Contains(review))
                throw new ArgumentException("Review already exists");
        
            if (review.GetUser() == null)
                throw new ArgumentException("User cannot be null");
        
            if (string.IsNullOrEmpty(review.GetMessage()))
                throw new ArgumentException("Message cannot be null or empty");
        
            if (review.GetDutyFreeRating() < MinRating || review.GetDutyFreeRating() > MaxRating)
                throw new ArgumentException($"Duty Free Rating must be between {MinRating} and {MaxRating}");
        
            if (review.GetFlightExperienceRating() < MinRating || review.GetFlightExperienceRating() > MaxRating)
                throw new ArgumentException($"Flight Experience Rating must be between {MinRating} and {MaxRating}");
        
            if (review.GetStaffFriendlinessRating() < MinRating || review.GetStaffFriendlinessRating() > MaxRating)
                throw new ArgumentException($"Staff Friendliness Rating must be between {MinRating} and {MaxRating}");
        
            if (review.GetCleanlinessRating() < MinRating || review.GetCleanlinessRating() > MaxRating)
                throw new ArgumentException($"Cleanliness Rating must be between {MinRating} and {MaxRating}");
        }
        
        public float CalculateAverageRating(Review review)
        {
            return (review.GetDutyFreeRating() +
                    review.GetFlightExperienceRating() +
                    review.GetStaffFriendlinessRating() +
                    review.GetCleanlinessRating()) / (float)NumberOfRatings;
        }
    }
}
