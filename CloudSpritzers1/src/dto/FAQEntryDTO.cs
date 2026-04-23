using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudSpritzers1.src.model.faq;

namespace CloudSpritzers1.src.dto
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

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
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

        private bool _hasFeedback;
        public bool HasFeedback
        {
            get => _hasFeedback;
            set
            {
                if (_hasFeedback != value)
                {
                    _hasFeedback = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isHelpfulSelected;
        public bool IsHelpfulSelected
        {
            get => _isHelpfulSelected;
            set
            {
                if (_isHelpfulSelected != value)
                {
                    _isHelpfulSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isNotHelpfulSelected;
        public bool IsNotHelpfulSelected
        {
            get => _isNotHelpfulSelected;
            set
            {
                if (_isNotHelpfulSelected != value)
                {
                    _isNotHelpfulSelected = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}