using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CloudSpritzers1.Src.ViewModel.Review
{
    public partial class AllReviewsViewModel : ObservableObject
    {
        private readonly ReviewService reviewService;
        private readonly IMapper mapper;

        public ObservableCollection<ReviewDTO> Reviews { get; } = new ();

        [ObservableProperty]
        private int totalReviews;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageDutyFree))]
        private double averageDutyFree;
        public string FormattedAverageDutyFree => AverageDutyFree.ToString("0.0");

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageFlightExperience))]
        private double averageFlightExperience;
        public string FormattedAverageFlightExperience => AverageFlightExperience.ToString("0.0");

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageStaffFriendliness))]
        private double averageStaffFriendliness;
        public string FormattedAverageStaffFriendliness => AverageStaffFriendliness.ToString("0.0");

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedAverageCleanliness))]
        private double averageCleanliness;
        public string FormattedAverageCleanliness => AverageCleanliness.ToString("0.0");

        public AllReviewsViewModel(ReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;
            LoadData();
        }

        public void LoadData()
        {
            var reviewsFromDb = reviewService.GetAll();
            Reviews.Clear();

            if (reviewsFromDb == null || reviewsFromDb.Count == 0)
            {
                return;
            }

            TotalReviews = reviewsFromDb.Count;

            CalculateCategoryAverages(reviewsFromDb);

            var mappedReviews = mapper.Map<List<ReviewDTO>>(reviewsFromDb);

            foreach (var reviewDataTransferObject in mappedReviews)
            {
                Reviews.Add(reviewDataTransferObject);
            }
        }

        private void CalculateCategoryAverages(List<Model.Review.Review> reviews)
        {
            AverageDutyFree = reviews.Average(review => review.GetDutyFreeRating());
            AverageFlightExperience = reviews.Average(review => review.GetFlightExperienceRating());
            AverageStaffFriendliness = reviews.Average(review => review.GetStaffFriendlinessRating());
            AverageCleanliness = reviews.Average(review => review.GetCleanlinessRating());
        }
    }
}
