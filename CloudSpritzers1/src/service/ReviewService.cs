using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Repository;

namespace CloudSpritzers1.Src.Service
{
    public class ReviewService
    {
        private readonly IRepository<int, Review> reviewRepository;

        private const int MinRating = 1;
        private const int MaxRating = 5;
        private const int NumberOfRatings = 4;

        public ReviewService(IRepository<int, Review> reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }

        public Review GetById(int id)
        {
            return reviewRepository.GetById(id);
        }

        public int Add(Review review)
        {
            return reviewRepository.CreateNewEntity(review);
        }

        public void UpdateById(int id, Review review)
        {
            reviewRepository.UpdateById(id, review);
        }

        public void DeleteById(int id)
        {
            reviewRepository.DeleteById(id);
        }

        public List<Review>? GetAll()
        {
            var reviews = reviewRepository.GetAll();
            return reviews?.ToList();
        }

        public void CreateReview(int id, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinessRating, int cleanlinessRating)
        {
            Review review = new (id, user, message, dutyFreeRating, flightExperienceRating, staffFriendlinessRating, cleanlinessRating);
            ValidateReview(review);
            Add(review);
        }

        public void ValidateReview(Review review)
        {
            ArgumentNullException.ThrowIfNull(review);

            if (this.GetAll().Contains(review))
            {
                throw new ArgumentException("Review already exists");
            }

            if (review.GetUser() == null)
            {
                throw new ArgumentException("User cannot be null");
            }

            if (string.IsNullOrEmpty(review.GetMessage()))
            {
                throw new ArgumentException("Message cannot be null or empty");
            }

            if (review.GetDutyFreeRating() < MinRating || review.GetDutyFreeRating() > MaxRating)
            {
                throw new ArgumentException($"Duty Free Rating must be between {MinRating} and {MaxRating}");
            }

            if (review.GetFlightExperienceRating() < MinRating || review.GetFlightExperienceRating() > MaxRating)
            {
                throw new ArgumentException($"Flight Experience Rating must be between {MinRating} and {MaxRating}");
            }

            if (review.GetStaffFriendlinessRating() < MinRating || review.GetStaffFriendlinessRating() > MaxRating)
            {
                throw new ArgumentException($"Staff Friendliness Rating must be between {MinRating} and {MaxRating}");
            }

            if (review.GetCleanlinessRating() < MinRating || review.GetCleanlinessRating() > MaxRating)
            {
                throw new ArgumentException($"Cleanliness Rating must be between {MinRating} and {MaxRating}");
            }
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
