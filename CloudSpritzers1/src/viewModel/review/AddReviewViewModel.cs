using System;
using System.Diagnostics;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CloudSpritzers1.Src.ViewModel.Review
{
    public partial class AddReviewViewModel : ObservableObject
    {
        private readonly ReviewService reviewService;

        public event EventHandler<(string Title, string Message)>? AlertRequested;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(DutyText))]
        private int dutyRating;
        public string DutyText => DutyRating > 0 ? $"{DutyRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(FlightText))]
        private int flightRating;
        public string FlightText => FlightRating > 0 ? $"{FlightRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(StaffText))]
        private int staffRating;
        public string StaffText => StaffRating > 0 ? $"{StaffRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(CleanText))]
        private int cleanRating;
        public string CleanText => CleanRating > 0 ? $"{CleanRating}/5" : "Not rated";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitReviewCommand))]
        [NotifyPropertyChangedFor(nameof(CharCountText))]
        private string reviewMessage = string.Empty;
        public string CharCountText => $"{ReviewMessage?.Length ?? 0} characters";

        public AddReviewViewModel(ReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        private bool CanSubmitReview()
        {
            return DutyRating > 0 &&
                   FlightRating > 0 &&
                   StaffRating > 0 &&
                   CleanRating > 0 &&
                   !string.IsNullOrWhiteSpace(ReviewMessage);
        }

        [RelayCommand(CanExecute = nameof(CanSubmitReview))]
        private void SubmitReview()
        {
            try
            {
                var app = App.Current as App;
                User? currentUser = app?.User;

                if (currentUser == null)
                {
                    AlertRequested?.Invoke(this, ("Not Logged In", "Oopsie Daisy! You must be logged in to leave a review"));
                    return;
                }

                reviewService.CreateReview(1, currentUser, ReviewMessage, DutyRating, FlightRating, StaffRating, CleanRating);

                DutyRating = 0;
                FlightRating = 0;
                StaffRating = 0;
                CleanRating = 0;
                ReviewMessage = string.Empty;

                AlertRequested?.Invoke(this, ("Success", "Your review has been successfully submitted!"));
            }
            catch (Exception ex)
            {
                AlertRequested?.Invoke(this, ("Oopsie Daisy! Error", $"We couldn't submit your review: {ex.Message}"));
            }
        }
    }
}