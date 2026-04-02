using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository.database;
using CloudSpritzers1.src.service;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.service;
using AutoMapper;
using System.Collections.ObjectModel;
using System.Linq;

namespace CloudSpritzers1.src.viewModel
{
    public class TicketsViewModel
    {
        private readonly TicketService _ticketService;
        private readonly IMapper _mapper;

        public ObservableCollection<TicketDTO> AllTickets { get; } = new();

        private ObservableCollection<TicketDTO> _ticketsRead = new();
        public ObservableCollection<TicketDTO> TicketsRead => _ticketsRead;

        private TicketFilter _selectedFilter = TicketFilter.ALL;

        private readonly TicketCategoryService _categoryService;
        private readonly TicketSubcategoryService _subcategoryService;
        private readonly UserService _userService;
        public ObservableCollection<TicketCategory> Categories { get; } = new();
        public ObservableCollection<TicketSubcategory> Subcategories { get; } = new();

       
        public TicketsViewModel(TicketService ticketService, TicketCategoryService categoryService, TicketSubcategoryService subcategoryService, UserService userService, IMapper mapper)
        {
            _ticketService = ticketService;
            _categoryService = categoryService;
            _subcategoryService = subcategoryService;
            _userService = userService;
            _mapper = mapper;

            LoadTickets();
            LoadCategories();
        }

        // =================================
        // PUBLIC API (UNCHANGED)
        // =================================
        public IEnumerable<TicketDTO> GetAllTickets()
        {
            return AllTickets;
        }

        public int NrTickets()
        {
            return AllTickets.Count;
        }

        public TicketFilter SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    ApplyFilter();
                }
            }
        }

        public string SelectedFilterString
        {
            get => SelectedFilter.ToString();
            set
            {
                if (Enum.TryParse<TicketFilter>(value, out var filter))
                    SelectedFilter = filter;
            }
        }

        // =================================
        // LOAD FROM DATABASE
        // =================================
        private void LoadTickets()
        {
            var ticketsFromDb = _ticketService.GetAllTickets();

            AllTickets.Clear();

            foreach (var ticket in ticketsFromDb)
            {
                var dto = _mapper.Map<TicketDTO>(ticket);
                AllTickets.Add(dto);
            }

            ApplyFilter();
        }

        // =================================
        // FILTER
        // =================================
        private void ApplyFilter()
        {
            _ticketsRead.Clear();

            IEnumerable<TicketDTO> filtered = AllTickets;

            switch (SelectedFilter)
            {
                case TicketFilter.OPEN:
                    filtered = AllTickets.Where(t => t.Status == StatusEnum.OPEN);
                    break;

                case TicketFilter.IN_PROGRESS:
                    filtered = AllTickets.Where(t => t.Status == StatusEnum.IN_PROGRESS);
                    break;

                case TicketFilter.RESOLVED:
                    filtered = AllTickets.Where(t => t.Status == StatusEnum.RESOLVED);
                    break;
            }

            foreach (var t in filtered)
                _ticketsRead.Add(t);
        }

        // =================================
        // UPDATE STATUS
        // =================================
        public void UpdateStatus(int ticketId, StatusEnum status)
        {
            _ticketService.UpdateStatus(ticketId, status);
            LoadTickets();
        }

        // =================================
        // UPDATE URGENCY
        // =================================
        public void UpdateUrgency(int ticketId, UrgencyLevelEnum urgency)
        {
            _ticketService.UpdateUrgencyLevel(ticketId, urgency);
            LoadTickets();
        }

        // =================================
        // CREATE TICKET
        // =================================
        public void CreateTicket(TicketDTO ticketDTO)
        {
            // Fetch related entities from DB
            var user = _userService.GetById(ticketDTO.UserId);
            var category = _categoryService.GetCategoryById(ticketDTO.CategoryId);
            var subcategory = _subcategoryService.GetSubcategoryById(ticketDTO.SubcategoryId);

            var ticket = new Ticket(
                ticketDTO.TicketId,
                user,
                ticketDTO.Status,
                category,
                subcategory,
                ticketDTO.Subject,
                ticketDTO.Description,
                ticketDTO.CreatedAt,
                ticketDTO.UrgencyLevel
            );

            _ticketService.AddTicket(ticket);
            LoadTickets();
        }
        //public void CreateTicket(TicketDTO ticketDTO)
        //{
        //    var ticket = _mapper.Map<Ticket>(ticketDTO);

        //    _ticketService.AddTicket(ticket);

        //    LoadTickets();
        //}
        private void LoadCategories()
        {
            Categories.Clear();
            foreach (var cat in _categoryService.GetAllCategories())
                Categories.Add(cat);
        }

        public void LoadSubcategories(int categoryId)
        {
            Subcategories.Clear();
            foreach (var sub in _subcategoryService.GetSubcategoriesByCategoryId(categoryId))
                Subcategories.Add(sub);
        }
    }

    public enum TicketFilter
    {
        ALL,
        OPEN,
        IN_PROGRESS,
        RESOLVED
    }
}