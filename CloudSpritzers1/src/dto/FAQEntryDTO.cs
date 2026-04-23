using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudSpritzers1.Src.Model.Faq;

namespace CloudSpritzers1.Src.Dto
{
    public class FAQEntryDTO : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public FAQCategoryEnum Category { get; set; }
        public int ViewCount { get; set; }
        public int HelpfulVotesCount { get; set; }
        public int NotHelpfulVotesCount { get; set; }

        private bool isExpanded;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        public FAQEntryDTO(
            int id,
            string question,
            string answer,
            FAQCategoryEnum category,
            int viewCount,
            int wasHelpfulVotes,
            int wasNotHelpfulVotes)
        {
            Id = id;
            Question = question;
            Answer = answer;
            Category = category;
            ViewCount = viewCount;
            HelpfulVotesCount = wasHelpfulVotes;
            NotHelpfulVotesCount = wasNotHelpfulVotes;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool hasFeedback;
        public bool HasFeedback
        {
            get => hasFeedback;
            set
            {
                if (hasFeedback != value)
                {
                    hasFeedback = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isHelpfulSelected;
        public bool IsHelpfulSelected
        {
            get => isHelpfulSelected;
            set
            {
                if (isHelpfulSelected != value)
                {
                    isHelpfulSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isNotHelpfulSelected;
        public bool IsNotHelpfulSelected
        {
            get => isNotHelpfulSelected;
            set
            {
                if (isNotHelpfulSelected != value)
                {
                    isNotHelpfulSelected = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}