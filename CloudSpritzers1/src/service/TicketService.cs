using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using User = CloudSpritzers1.src.model.User;

namespace CloudSpritzers1.src.service
{
    public class TicketService
    {
        private readonly TicketRepository _ticketRepository;

        public TicketService(TicketRepository repository)
        {
            _ticketRepository = repository;
        }

        public void CreateTicket(int ticketId, User user, StatusEnum status, TicketCategory category, TicketSubcategory subcategory, string subject, string description, DateTime createdAt, UrgencyLevelEnum? urgencyLevel = null)
        {  
            Ticket ticket = new Ticket(ticketId, user, status, category, subcategory, subject, description, createdAt, urgencyLevel);
            ValidateTicket(ticket);
            AddTicket(ticket);
        }

        public void AddTicket(Ticket ticket)
        {
            _ticketRepository.Add(ticket);
        }
        public void DeleteTicket(int ticketId)
        {
            _ticketRepository.DeleteById(ticketId);
        }
        public Ticket GetTicket(int ticketId)
        {
            return _ticketRepository.GetById(ticketId);
        }

        public IEnumerable<Ticket> GetAllTickets()
        {
            return _ticketRepository.GetAll();
        }
        public void UpdateById(int id, Ticket ticket)
        {
            _ticketRepository.UpdateById(id, ticket);
        }
        
        public void ValidateTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException("The ticket does not have any data.");
            if (ticket.User == null)
                throw new ArgumentNullException("The user does not have any data.");
            if (ticket.Category == null)
                throw new ArgumentNullException("Null Category.");
            if (ticket.Subcategory == null)
                throw new ArgumentNullException("Null Subcategory.");
            if (ticket.Subcategory.Category.CategoryId != ticket.Category.CategoryId)
                throw new ArgumentException($"The subcategory '{ticket.Subcategory.SubcategoryName}' does not belong to the category '{ticket.Category.Name}'");
            if (string.IsNullOrWhiteSpace(ticket.Subject))
                throw new ArgumentNullException("The Subject is empty.");
            if (string.IsNullOrWhiteSpace(ticket.Description))
                throw new ArgumentNullException("The Description is empty.");
        }

        public void UpdateUrgencyLevel(int ticketId, UrgencyLevelEnum urgencyLevel)
        {
            Ticket elem = _ticketRepository.GetById(ticketId);
            elem.ChangeUrgencyLevel(urgencyLevel);
            _ticketRepository.UpdateById(ticketId, elem);
        }

        public void UpdateStatus(int ticketId, StatusEnum status)
        {
            Ticket elem = _ticketRepository.GetById(ticketId);
            elem.UpdateStatus(status);
            _ticketRepository.UpdateById(ticketId, elem);
        }

    }
}
