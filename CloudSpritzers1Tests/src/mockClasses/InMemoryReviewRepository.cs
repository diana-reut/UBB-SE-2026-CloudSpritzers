using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryReviewRepository : IRepository<int, Review>
    {
        private readonly List<Review> _reviews = new List<Review>();

        public int CreateNewEntity(Review reviewElement)
        {
            if (reviewElement == null)
                throw new ArgumentNullException(nameof(reviewElement), "Review cannot be null.");

            _reviews.Add(reviewElement);
            return reviewElement.GetId();
        }

        public void DeleteById(int id)
        {
            var review = _reviews.FirstOrDefault(r => r.GetId() == id);
            if (review != null)
            {
                _reviews.Remove(review);
            }

            
        }

        public IEnumerable<Review> GetAll() => _reviews;

        public Review GetById(int id)
        {
            var review = _reviews.FirstOrDefault(r => r.GetId() == id);
            if (review == null)
                throw new KeyNotFoundException($"Review with id {id} was not found.");

            return review;
        }

        public void UpdateById(int id, Review reviewElement)
        {
            if (reviewElement == null)
                throw new ArgumentNullException(nameof(reviewElement), "Review cannot be null.");

            var index = _reviews.FindIndex(r => r.GetId() == id);
            if (index == -1)
                throw new KeyNotFoundException($"Review with id {id} was not found.");

            _reviews[index] = reviewElement;
        }
    }
}