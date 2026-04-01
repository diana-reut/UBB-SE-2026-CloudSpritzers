using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.viewModel
{
    public class TicketsViewModel
    {
        private readonly TicketService _ticketService;

        private ObservableCollection<TicketDTO> AllTickets { get; } = new ObservableCollection<TicketDTO>();

        private ObservableCollection<TicketDTO> _ticketsRead = new ObservableCollection<TicketDTO>();
        public ObservableCollection<TicketDTO> TicketsRead => _ticketsRead;

        private TicketFilter _selectedFilter = TicketFilter.ALL;

        public TicketsViewModel(TicketService ticketService)
        {
            _ticketService = ticketService;


            CreateMockTicket();

            ApplyFilter();

            LoadTickets();
        }
        private void CreateMockTicket()
        {
            var user = new User(1, "John Doe", "john@example.com");
            var category = new TicketCategory(1, "Baggage", UrgencyLevelEnum.LOW);
            var subcategory = new TicketSubcategory(1, "Lost Items", 3, category);
            int ticketId = new Random().Next(100, 999);
            var status = StatusEnum.OPEN;
            string subject = "Lost baggage";
            string description = "My suitcase did not arrive on time.";
            var createdAt = DateTime.Now;

            _ticketService.CreateTicket(ticketId, user, status, category, subcategory, subject, description, createdAt);

            AllTickets.Add(MapToDTO(_ticketService.GetTicket(ticketId)));
        }
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

        private void LoadTickets()
        {
            AllTickets.Clear();
            foreach (var ticket in _ticketService.GetAllTickets())
                AllTickets.Add(MapToDTO(ticket));

            ApplyFilter();
        }

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

        private TicketDTO MapToDTO(Ticket ticket)
        {
            return new TicketDTO(
                TicketId: ticket.TicketId,
                UserId: ticket.User.GetId(),
                UserEmail: ticket.User.GetEmail(),
                UrgencyLevel: ticket.UrgencyLevel,
                Status: ticket.Status,
                CategoryId: ticket.Category.CategoryId,
                CategoryName: ticket.Category.Name,
                SubcategoryId: ticket.Subcategory.SubcategoryId,
                SubcategoryName: ticket.Subcategory.SubcategoryName,
                Subject: ticket.Subject,
                Description: ticket.Description,
                CreatedAt: ticket.CreatedAt
            );
        }

        public void UpdateStatus(int ticketId, StatusEnum status)
        {
            _ticketService.UpdateStatus(ticketId, status);

            var updatedDto = MapToDTO(_ticketService.GetTicket(ticketId));

            var mainIndex = AllTickets.IndexOf(AllTickets.First(t => t.TicketId == ticketId));
            if (mainIndex >= 0)
                AllTickets[mainIndex] = updatedDto;

            ApplyFilter();
        }

        //not used but i will let it live for now
        public void UpdateUrgency(int ticketId, UrgencyLevelEnum urgency)
        {
            _ticketService.UpdateUrgencyLevel(ticketId, urgency);

            var updatedDto = MapToDTO(_ticketService.GetTicket(ticketId));
            var mainIndex = AllTickets.IndexOf(AllTickets.First(t => t.TicketId == ticketId));
            if (mainIndex >= 0)
                AllTickets[mainIndex] = updatedDto;

            ApplyFilter();
        }

        public void CreateTicket(TicketDTO ticketDTO)
        {
            //hardcoded user 
            var user = new User(1, "", ticketDTO.UserEmail);
            var category = new TicketCategory(ticketDTO.CategoryId, ticketDTO.CategoryName, UrgencyLevelEnum.LOW);
            var subcategory = new TicketSubcategory(ticketDTO.SubcategoryId, ticketDTO.SubcategoryName, 1, category);

            _ticketService.CreateTicket(ticketDTO.TicketId, user, StatusEnum.OPEN, category, subcategory,
                ticketDTO.Subject, ticketDTO.Description, ticketDTO.CreatedAt);

            AllTickets.Add(MapToDTO(_ticketService.GetTicket(ticketDTO.TicketId)));
            ApplyFilter();
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