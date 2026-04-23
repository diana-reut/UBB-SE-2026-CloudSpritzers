using AutoMapper;
using CloudSpritzers1.Src.Model.Review;
using CloudSpritzers1.Src.Dto;
using Microsoft.IdentityModel.Tokens;

namespace CloudSpritzers1.Src.Dto.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            System.Diagnostics.Debug.WriteLine("ReviewMappingProfile Loaded!");
            CreateMap<Review, ReviewDTO>()

            .ConstructUsing(src => new ReviewDTO(
                src.GetId(),
                src.GetUser().UserId,
                src.GetUser().RetrieveConfiguredDisplayFullNameForBot(),
                src.GetMessage(),
                src.GetDutyFreeRating(),
                src.GetFlightExperienceRating(),
                src.GetStaffFriendlinessRating(),
                src.GetCleanlinessRating(),
                CalculateOverallAverage(src))); // Replaces manual math in loop
        }

        private static float CalculateOverallAverage(Review review)
        {
            float sumOfRatings = review.GetDutyFreeRating() +
                                 review.GetFlightExperienceRating() +
                                 review.GetStaffFriendlinessRating() +
                                 review.GetCleanlinessRating();

            return sumOfRatings / 4.0f;
        }
    }
}
