using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository.Database;
using CloudSpritzers1.Src.Service;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Service;
using AutoMapper;

namespace CloudSpritzers1.Src.ViewModel
{
    public class TicketsViewModel
    {
        private readonly ITicketService ticketService;
        private readonly IMapper mapper;
        private readonly ITicketCategoryService categoryService;
        private readonly ITicketSubcategoryService subcategoryService;
        private readonly IUserService userService;

        public ObservableCollection<TicketDTO> AllTickets { get; } = new ();

        private ObservableCollection<TicketDTO> filteredTicketsForDisplay = new ();
        public ObservableCollection<TicketDTO> FilteredTicketsForDisplay => filteredTicketsForDisplay;

        private TicketFilterStatusEnum selectedFilter = TicketFilterStatusEnum.ALL;

        public ObservableCollection<TicketCategory> Categories { get; } = new ();
        public ObservableCollection<TicketSubcategory> Subcategories { get; } = new ();

        public TicketsViewModel(ITicketService ticketService, ITicketCategoryService categoryService, ITicketSubcategoryService subcategoryService, IUserService userService, IMapper mapper)
        {
            this.ticketService = ticketService;
            this.categoryService = categoryService;
            this.subcategoryService = subcategoryService;
            this.userService = userService;
            this.mapper = mapper;

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

        public int GetTotalTicketCount()
        {
            return AllTickets.Count;
        }

        public TicketFilterStatusEnum SelectedFilterStatus
        {
            get => selectedFilter;
            set
            {
                if (selectedFilter != value)
                {
                    selectedFilter = value;
                    ApplyFilterLogic();
                }
            }
        }

        public string SelectedFilterString
        {
            get => SelectedFilterStatus.ToString();
            set
            {
                if (Enum.TryParse<TicketFilterStatusEnum>(value, out var filter))
                {
                    SelectedFilterStatus = filter;
                }
            }
        }

        // =================================
        // LOAD FROM DATABASE
        // =================================
        private void LoadTickets()
        {
            var ticketsFromDatabase = ticketService.GetAllTickets();

            AllTickets.Clear();

            foreach (var ticketEntity in ticketsFromDatabase)
            {
                var ticketDTO = mapper.Map<TicketDTO>(ticketEntity);
                AllTickets.Add(ticketDTO);
            }

            ApplyFilterLogic();
        }

        // =================================
        // FILTER
        // =================================
        private void ApplyFilterLogic()
        {
            filteredTicketsForDisplay.Clear();

            IEnumerable<TicketDTO> filteredResults = ticketService.FilterTicketsByStatus(
                AllTickets,
                SelectedFilterStatus);

            foreach (var ticket in filteredResults)
            {
                filteredTicketsForDisplay.Add(ticket);
            }
        }

        // =================================
        // UPDATE STATUS
        // =================================
        public void UpdateStatus(int ticketId, TicketStatusEnum newStatus)
        {
            ticketService.UpdateStatus(ticketId, newStatus);
            LoadTickets();
        }

        // =================================
        // UPDATE URGENCY
        // =================================
        public void UpdateUrgencyLevel(int ticketId, TicketUrgencyLevelEnum newUrgencyLevel)
        {
            ticketService.UpdateUrgencyLevel(ticketId, newUrgencyLevel);
            LoadTickets();
        }

        // =================================
        // CREATE TICKET
        // =================================
        public void CreateTicket(TicketDTO ticketDTO)
        {
            // Fetch related entities from DB
            var creator = userService.GetById(ticketDTO.creatorAccountId);
            var category = categoryService.GetCategoryById(ticketDTO.categoryId);
            var subcategory = subcategoryService.GetSubcategoryById(ticketDTO.subcategoryId);

            var ticket = new Ticket(
                ticketDTO.ticketId,
                creator,
                ticketDTO.currentStatus,
                category,
                subcategory,
                ticketDTO.subject,
                ticketDTO.description,
                ticketDTO.creationTimestamp,
                ticketDTO.urgencyLevel);

            ticketService.AddTicket(ticket);
            LoadTickets();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            foreach (var categoryEntity in categoryService.GetAllCategories())
            {
                Categories.Add(categoryEntity);
            }
        }

        public void LoadSubcategories(int categoryId)
        {
            Subcategories.Clear();
            foreach (var subcategoryEntity in subcategoryService.GetSubcategoriesByCategoryId(categoryId))
            {
                Subcategories.Add(subcategoryEntity);
            }
        }
    }

    public enum TicketFilterStatusEnum
    {
        ALL,
        OPEN,
        IN_PROGRESS,
        RESOLVED
    }
}