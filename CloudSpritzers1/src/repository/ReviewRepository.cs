using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.review;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.src.repository.interfaces;
using CloudSpritzers1.src.repository.database;

namespace CloudSpritzers1.src.repository
{
    public class ReviewRepository : DatabaseRepository<int, Review>, IRepository<int, Review>
    {
        //private UserRepository _userRepository = new UserRepository();

        //public ReviewRepository() { }

        private readonly IRepository<int, User> _userRepository;

        // Dependency Injection: Pass the repository in rather than creating it here
        public ReviewRepository(IRepository<int, User> userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }



        public Review GetById(int reviewId)
        {
            string query = "SELECT * FROM Review WHERE review_id = @id";
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@id", reviewId);

            Review review = base.GetById(reviewId, command);

            if (review == null)
                throw new KeyNotFoundException($"Review with id {reviewId} was not found.");

            return review;
        }

        public IEnumerable<Review> GetAll()
        {
            string query = "SELECT * FROM Review";
            SqlCommand command = new SqlCommand(query);
            return base.GetAll(command);
        }

        public int CreateNewEntity(Review reviewElement)
        {
            if (reviewElement == null)
                throw new ArgumentNullException(nameof(reviewElement), "Review cannot be null.");

            string query = "INSERT INTO Review " +
                "(user_id, message, duty_free_rating, flight_experience_rating, staff_friendliness_rating, cleanliness_rating) " +
                "OUTPUT INSERTED.review_id " +
                "VALUES (@userId, @message, @dutyFree, @flightExp, @staff, @clean)";

            SqlCommand command = new SqlCommand(query);

            command.Parameters.AddWithValue("@userId", reviewElement.GetUser().UserId);
            command.Parameters.AddWithValue("@message", reviewElement.GetMessage());
            command.Parameters.AddWithValue("@dutyFree", reviewElement.GetDutyFreeRating());
            command.Parameters.AddWithValue("@flightExp", reviewElement.GetFlightExperienceRating());
            command.Parameters.AddWithValue("@staff", reviewElement.GetStaffFriendlinessRating());
            command.Parameters.AddWithValue("@clean", reviewElement.GetCleanlinessRating());

            int id = base.Add(command, reviewElement);
            return id;
        }

        public void UpdateById(int id, Review reviewElement)
        {
            if (reviewElement == null)
                throw new ArgumentNullException(nameof(reviewElement), "Review cannot be null.");

            string query = "UPDATE Review SET " +
                "user_id = @userId, " +
                "message = @message, " +
                "duty_free_rating = @dutyFree, " +
                "flight_experience_rating = @flightExp, " +
                "staff_friendliness_rating = @staff, " +
                "cleanliness_rating = @clean " +
                "WHERE review_id = @id";

            SqlCommand command = new SqlCommand(query);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@userId", reviewElement.GetUser().UserId);
            command.Parameters.AddWithValue("@message", reviewElement.GetMessage());
            command.Parameters.AddWithValue("@dutyFree", reviewElement.GetDutyFreeRating());
            command.Parameters.AddWithValue("@flightExp", reviewElement.GetFlightExperienceRating());
            command.Parameters.AddWithValue("@staff", reviewElement.GetStaffFriendlinessRating());
            command.Parameters.AddWithValue("@clean", reviewElement.GetCleanlinessRating());

            base.UpdateById(id, command, reviewElement);
        }

        public void DeleteById(int reviewId)
        {
            string query = "DELETE FROM Review WHERE review_id = @id";
            SqlCommand command = new SqlCommand(query);
            command.Parameters.AddWithValue("@id", reviewId);

            base.DeleteById(reviewId, command);
        }

        protected override Review MapRowToEntity(SqlDataReader reader)
        {
            int reviewId = reader.GetInt32(reader.GetOrdinal("review_id"));
            int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
            string reviewMessage = reader.GetString(reader.GetOrdinal("message"));
            int dutyFreeRating = reader.GetInt32(reader.GetOrdinal("duty_free_rating"));
            int flightExperienceRating = reader.GetInt32(reader.GetOrdinal("flight_experience_rating"));
            int staffFriendlinessRating = reader.GetInt32(reader.GetOrdinal("staff_friendliness_rating"));
            int cleanlinessRating = reader.GetInt32(reader.GetOrdinal("cleanliness_rating"));

            User reviewUser = _userRepository.GetById(userId);

            return new Review(reviewId, reviewUser, reviewMessage, dutyFreeRating, flightExperienceRating, staffFriendlinessRating, cleanlinessRating);
        }

        protected override int GetEntityId(Review entity)
        {
            return entity.GetId();
        }

    }
}