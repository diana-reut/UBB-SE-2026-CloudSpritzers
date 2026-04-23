using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model.faq;
using CloudSpritzers1.src.service.implementation;
using CloudSpritzers1.src.service.interfaces;
using CloudSpritzers1.src.view.faq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.viewModel.faq
{
    public class FAQViewModel : INotifyPropertyChanged
    {
        private readonly IFAQService _faqService;
        private readonly IMapper _mapper;

        private ObservableCollection<FAQEntryDTO> _faqs;
        private ObservableCollection<FAQEntryDTO> _filteredFAQs;
        private FAQEntryDTO? _selectedFAQEntry;
        private string _searchQuery;
        private FAQCategoryEnum _selectedCategory;
        private bool _isAdmin;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<FAQEntryDTO> FAQs
        {
            get => _faqs;
            set
            {
                _faqs = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FAQEntryDTO> FilteredFAQs
        {
            get => _filteredFAQs;
            set
            {
                _filteredFAQs = value;
                OnPropertyChanged();
            }
        }

        public FAQEntryDTO? SelectedFAQEntry
        {
            get => _selectedFAQEntry;
            set
            {
                _selectedFAQEntry = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public FAQCategoryEnum SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public FAQViewModel(IFAQService faqService, IMapper mapper)
        {
            _faqService = faqService;
            _mapper = mapper;

            _faqs = new ObservableCollection<FAQEntryDTO>();
            _filteredFAQs = new ObservableCollection<FAQEntryDTO>();
            _searchQuery = string.Empty;
            _selectedCategory = FAQCategoryEnum.All;
        }

        public void LoadFAQ()
        {
            FAQs.Clear();

            var entries = _faqService.GetAll().OrderByDescending(entry => entry.ViewCount);
            foreach (var entry in entries)
            {
                FAQs.Add(_mapper.Map<FAQEntryDTO>(entry));
            }

            ApplyFilters();
        }

        public void ApplyFilters()
        {
            var result = _faqService.FilterFAQEntry(SelectedCategory, SearchQuery)
                                    .OrderByDescending(entry => entry.ViewCount)
                                    .AsEnumerable().Select(entry => _mapper.Map<FAQEntryDTO>(entry));

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

        //public void Search()
        //{
        //    ApplyFilters();
        //}

        public void AddFAQEntry(FAQEntryDTO faqDto)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Only admins can add FAQs.");

            var entity = _mapper.Map<FAQEntry>(faqDto);
            _faqService.AddFAQEntry(entity);
            LoadFAQ();
        }

        public void EditFAQEntry(FAQEntryDTO faqDto)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Only admins can edit FAQs.");

            if (faqDto == null)
                throw new ArgumentNullException(nameof(faqDto));

            var entity = _mapper.Map<FAQEntry>(faqDto);
            _faqService.EditFAQEntry(entity, faqDto.Id);
            LoadFAQ();
        }

        public void DeleteFAQEntry(FAQEntryDTO faqDto)
        {
            if (!IsAdmin)
                throw new UnauthorizedAccessException("Only admins can delete FAQs.");

            if (faqDto == null)
                throw new ArgumentNullException(nameof(faqDto));

            _faqService.DeleteFAQEntry(faqDto.Id);
            LoadFAQ();
        }

        public void IncrementViewCount()
        {
            if (SelectedFAQEntry == null)
                return;

            var entity = _mapper.Map<FAQEntry>(SelectedFAQEntry);
            _faqService.IncrementViewCount(entity);
            LoadFAQ();
        }

        public void IncrementWasHelpfulVotes()
        {
            if (SelectedFAQEntry == null)
                return;

            var entity = _mapper.Map<FAQEntry>(SelectedFAQEntry);
            _faqService.IncrementWasHelpfulVotes(entity);

            SelectedFAQEntry.HelpfulVotesCount++;
            OnPropertyChanged(nameof(SelectedFAQEntry));
        }

        public void IncrementWasNotHelpfulVotes()
        {
            if (SelectedFAQEntry == null)
                return;

            var entity = _mapper.Map<FAQEntry>(SelectedFAQEntry);
            _faqService.IncrementWasNotHelpfulVotes(entity);

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
                return;

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
                return;

            var entity = _mapper.Map<FAQEntry>(faq);
            _faqService.IncrementViewCount(entity);

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
                throw new ArgumentException("Question cannot be empty.");

            if (string.IsNullOrWhiteSpace(answer))
                throw new ArgumentException("Answer cannot be empty.");

            if (!Enum.TryParse<FAQCategoryEnum>(categoryString, out var category))
                throw new ArgumentException("Invalid category.");

            var dto = new FAQEntryDTO(
                SelectedFAQEntry?.Id ?? 0,
                question.Trim(),
                answer.Trim(),
                category,
                SelectedFAQEntry?.ViewCount ?? 0,
                SelectedFAQEntry?.HelpfulVotesCount ?? 0,
                SelectedFAQEntry?.NotHelpfulVotesCount ?? 0
            );

            if (dto.Id == 0)
                AddFAQEntry(dto);
            else
                EditFAQEntry(dto);

            return Task.CompletedTask;
        }

        public void SetCategory(FAQCategoryEnum category)
        {
            SelectedCategory = category;
            ApplyFilters();
        }

        public void GiveFeedback(FAQEntryDTO faq, bool isHelpful)
        {
            if (faq == null) return;

            SelectedFAQEntry = faq;

            var entity = _mapper.Map<FAQEntry>(faq);

            if (isHelpful)
            {
                _faqService.IncrementWasHelpfulVotes(entity);
                faq.HelpfulVotesCount++;
            }
            else
            {
                _faqService.IncrementWasNotHelpfulVotes(entity);
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