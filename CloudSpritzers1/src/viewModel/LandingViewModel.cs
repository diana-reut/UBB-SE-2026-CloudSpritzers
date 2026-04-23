using System.Collections.ObjectModel;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CloudSpritzers1.Src.ViewModel
{
    public partial class LandingViewModel : ObservableObject
    {
        private readonly ReviewService reviewService;
        private readonly IMapper mapper;

        public ObservableCollection<ReviewDTO> Reviews { get; } = new ();

        public LandingViewModel(ReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;
            LoadReviews();
        }

        public void LoadReviews()
        {
            var allReviews = reviewService.GetAll();
            Reviews.Clear();

            foreach (var review in allReviews)
            {
                string realName = review.GetUser().RetrieveConfiguredDisplayFullNameForBot();

                float averageRating = reviewService.CalculateAverageRating(review);

                var reviewDto = mapper.Map<ReviewDTO>(review);

                var finalDto = reviewDto with
                {
                    userName = realName,
                    overallRating = averageRating
                };

                Reviews.Add(finalDto);
            }
        }
    }
}