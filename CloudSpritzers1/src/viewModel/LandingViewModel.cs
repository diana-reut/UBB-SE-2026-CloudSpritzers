using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.service;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace CloudSpritzers1.src.viewmodel
{
    public partial class LandingViewModel : ObservableObject
    {
        private readonly ReviewService _reviewService;
        private readonly IMapper _mapper;

        public ObservableCollection<ReviewDTO> Reviews { get; } = new();

        public LandingViewModel(ReviewService reviewService, IMapper mapper)
        {
            _reviewService = reviewService;
            _mapper = mapper;
            LoadReviews();
        }

        public void LoadReviews()
        {
            var allReviews = _reviewService.GetAll();
            Reviews.Clear();
    
            foreach (var review in allReviews)
            {
                string realName = review.GetUser().GetFullName();

                float averageRating = _reviewService.CalculateAverageRating(review);

                var reviewDto = _mapper.Map<ReviewDTO>(review);

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