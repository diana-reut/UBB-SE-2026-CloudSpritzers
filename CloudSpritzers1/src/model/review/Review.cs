using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1.Src.Model.Review
{
    public class Review
    {
        private int id;
        private User user;
        private string message;
        private int dutyFreeRating;
        private int flightExperienceRating;
        private int staffFriendlinessRating;
        private int cleanlinessRating;

        public Review(int id, User user, string message, int dutyFreeRating, int flightExperienceRating, int staffFriendlinesRating, int cleanlinessRating)
        {
            this.id = id;
            this.user = user;
            this.message = message;
            this.dutyFreeRating = dutyFreeRating;
            this.flightExperienceRating = flightExperienceRating;
            this.staffFriendlinessRating = staffFriendlinesRating;
            this.cleanlinessRating = cleanlinessRating;
        }

        // GETTERS
        public int GetId()
        {
            return this.id;
        }

        public User GetUser()
        {
            return this.user;
        }
        public string GetMessage()
        {
            return this.message;
        }
        public int GetDutyFreeRating()
        {
            return this.dutyFreeRating;
        }
        public int GetFlightExperienceRating()
        {
            return this.flightExperienceRating;
        }
        public int GetStaffFriendlinessRating()
        {
            return this.staffFriendlinessRating;
        }
        public int GetCleanlinessRating()
        {
            return this.cleanlinessRating;
        }
    }
}
