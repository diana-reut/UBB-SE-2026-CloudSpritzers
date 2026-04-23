using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Service.Implementation;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.View.Faq;

namespace CloudSpritzers1.Src.ViewModel.Faq
{
    public class FAQViewModel : INotifyPropertyChanged
    {
        private readonly IFAQService faqService;
        private readonly IMapper mapper;

        private ObservableCollection<FAQEntryDTO> faqs;
        private ObservableCollection<FAQEntryDTO> filteredFAQs;
        private FAQEntryDTO? selectedFAQEntry;
        private string searchQuery;
        private FAQCategoryEnum selectedCategory;
        private bool isAdmin;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<FAQEntryDTO> FAQs
        {
            get => faqs;
            set
            {
                faqs = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FAQEntryDTO> FilteredFAQs
        {
            get => filteredFAQs;
            set
            {
                filteredFAQs = value;
                OnPropertyChanged();
            }
        }

        public FAQEntryDTO? SelectedFAQEntry
        {
            get => selectedFAQEntry;
            set
            {
                selectedFAQEntry = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public FAQCategoryEnum SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public bool IsAdmin
        {
            get => isAdmin;
            set
            {
                isAdmin = value;
                OnPropertyChanged();
            }
        }

        public FAQViewModel(IFAQService faqService, IMapper mapper)
        {
            this.faqService = faqService;
            this.mapper = mapper;

            faqs = new ObservableCollection<FAQEntryDTO>();
            filteredFAQs = new ObservableCollection<FAQEntryDTO>();
            searchQuery = string.Empty;
            selectedCategory = FAQCategoryEnum.All;
        }

        public void LoadFAQ()
        {
            FAQs.Clear();

            var entries = faqService.GetAll().OrderByDescending(entry => entry.ViewCount);
            foreach (var entry in entries)
            {
                FAQs.Add(mapper.Map<FAQEntryDTO>(entry));
            }

            ApplyFilters();
        }

        public void ApplyFilters()
        {
            var result = faqService.FilterFAQEntry(SelectedCategory, SearchQuery)
                                    .OrderByDescending(entry => entry.ViewCount)
                                    .AsEnumerable().Select(entry => mapper.Map<FAQEntryDTO>(entry));

            FilteredFAQs.Clear();
            foreach (var faq in result)
            {
                FilteredFAQs.Add(faq);
            }
        }

        public void FilterByCategory(FAQCategoryEnum category)
        {
            SelectedCategory = category;
        }

        // public void Search()
        // {
        //    ApplyFilters();
        // }
        public void AddFAQEntry(FAQEntryDTO faqDto)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can add FAQs.");
            }

            var entity = mapper.Map<FAQEntry>(faqDto);
            faqService.AddFAQEntry(entity);
            LoadFAQ();
        }

        public void EditFAQEntry(FAQEntryDTO faqDto)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can edit FAQs.");
            }

            if (faqDto == null)
            {
                throw new ArgumentNullException(nameof(faqDto));
            }

            var entity = mapper.Map<FAQEntry>(faqDto);
            faqService.EditFAQEntry(entity, faqDto.Id);
            LoadFAQ();
        }

        public void DeleteFAQEntry(FAQEntryDTO faqDto)
        {
            if (!IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can delete FAQs.");
            }

            if (faqDto == null)
            {
                throw new ArgumentNullException(nameof(faqDto));
            }

            faqService.DeleteFAQEntry(faqDto.Id);
            LoadFAQ();
        }

        public void IncrementViewCount()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var entity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            faqService.IncrementViewCount(entity);
            LoadFAQ();
        }

        public void IncrementWasHelpfulVotes()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var entity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            faqService.IncrementWasHelpfulVotes(entity);

            SelectedFAQEntry.HelpfulVotesCount++;
            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        public void IncrementWasNotHelpfulVotes()
        {
            if (SelectedFAQEntry == null)
            {
                return;
            }

            var entity = mapper.Map<FAQEntry>(SelectedFAQEntry);
            faqService.IncrementWasNotHelpfulVotes(entity);

            SelectedFAQEntry.NotHelpfulVotesCount++;
            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ToggleFAQ(FAQEntryDTO faqDto)
        {
            if (faqDto == null)
            {
                return;
            }

            bool willExpand = !faqDto.IsExpanded;

            foreach (var faq in FilteredFAQs)
            {
                faq.IsExpanded = false;
            }

            faqDto.IsExpanded = willExpand;

            if (willExpand)
            {
                SelectedFAQEntry = faqDto;
                IncrementViewCountFor(faqDto.Id);
            }
            else
            {
                SelectedFAQEntry = null;
            }
        }

        public void IncrementViewCountFor(int faqId)
        {
            var faq = FAQs.FirstOrDefault(x => x.Id == faqId);
            if (faq == null)
            {
                return;
            }

            var entity = mapper.Map<FAQEntry>(faq);
            faqService.IncrementViewCount(entity);

            faq.ViewCount++;

            var filteredFaq = FilteredFAQs.FirstOrDefault(x => x.Id == faqId);
            if (filteredFaq != null && filteredFaq != faq)
            {
                filteredFaq.ViewCount = faq.ViewCount;
            }

            OnPropertyChanged(nameof(FAQs));
            OnPropertyChanged(nameof(FilteredFAQs));
        }

        public Task Save(string question, string answer, string? categoryString)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new ArgumentException("Answer cannot be empty.");
            }

            if (!Enum.TryParse<FAQCategoryEnum>(categoryString, out var category))
            {
                throw new ArgumentException("Invalid category.");
            }

            var dto = new FAQEntryDTO(
                SelectedFAQEntry?.Id ?? 0,
                question.Trim(),
                answer.Trim(),
                category,
                SelectedFAQEntry?.ViewCount ?? 0,
                SelectedFAQEntry?.HelpfulVotesCount ?? 0,
                SelectedFAQEntry?.NotHelpfulVotesCount ?? 0);

            if (dto.Id == 0)
            {
                AddFAQEntry(dto);
            }
            else
            {
                EditFAQEntry(dto);
            }

            return Task.CompletedTask;
        }

        public void SetCategory(FAQCategoryEnum category)
        {
            SelectedCategory = category;
            ApplyFilters();
        }

        public void GiveFeedback(FAQEntryDTO faq, bool isHelpful)
        {
            if (faq == null)
            {
                return;
            }

            SelectedFAQEntry = faq;

            var entity = mapper.Map<FAQEntry>(faq);

            if (isHelpful)
            {
                faqService.IncrementWasHelpfulVotes(entity);
                faq.HelpfulVotesCount++;
            }
            else
            {
                faqService.IncrementWasNotHelpfulVotes(entity);
                faq.NotHelpfulVotesCount++;
            }

            faq.HasFeedback = true;
            faq.IsHelpfulSelected = isHelpful;
            faq.IsNotHelpfulSelected = !isHelpful;

            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        public FAQNavigationData BuildNavigationData(int currentPersonId)
        {
            return new FAQNavigationData
            {
                CurrentPersonId = currentPersonId,
                IsEmployee = IsAdmin,
                FAQEntry = SelectedFAQEntry
            };
        }
    }
}