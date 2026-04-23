using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Interfaces;
using CloudSpritzers1.Src.Service.Interfaces;
using CloudSpritzers1.Src.ViewModel;
using Windows.System;
using User = CloudSpritzers1.Src.Model.User;

namespace CloudSpritzers1.Src.Service
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public void CreateTicket(int ticketId, User ticketCreator, TicketStatusEnum initialStatus, TicketCategory category, TicketSubcategory subcategory, string subject, string description, DateTime creationTimestamp, TicketUrgencyLevelEnum? initialUrgencyLevel = null)
        {
            Ticket newTicket = new Ticket(ticketId, ticketCreator, initialStatus, category, subcategory, subject, description, creationTimestamp, initialUrgencyLevel);

            ValidateTicket(newTicket);
            AddTicket(newTicket);
        }

        public void AddTicket(Ticket ticketEntity)
        {
            ticketRepository.CreateNewEntity(ticketEntity);
        }
        public void DeleteTicketById(int ticketId)
        {
            ticketRepository.DeleteById(ticketId);
        }
        public Ticket GetTicketById(int ticketId)
        {
            return ticketRepository.GetById(ticketId);
        }

        public IEnumerable<Ticket> GetAllTickets()
        {
            return ticketRepository.GetAll();
        }
        public void UpdateTicketById(int id, Ticket ticket)
        {
            ticketRepository.UpdateById(id, ticket);
        }
        public void ValidateTicket(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException("The newTicket does not have any data.");
            }
            if (ticket.Creator == null)
            {
                throw new ArgumentNullException("The ticketCreator does not have any data.");
            }
            if (ticket.Category == null)
            {
                throw new ArgumentNullException("Null Category.");
            }
            if (ticket.Subcategory == null)
            {
                throw new ArgumentNullException("Null Subcategory.");
            }
            if (ticket.Subcategory.ParentCategory.CategoryId != ticket.Category.CategoryId)
            {
                throw new ArgumentException($"The subcategory '{ticket.Subcategory.SubcategoryName}' does not belong to the category '{ticket.Category.CategoryName}'");
            }
            if (string.IsNullOrWhiteSpace(ticket.Subject))
            {
                throw new ArgumentNullException("The Subject is empty.");
            }
            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new ArgumentNullException("The Description is empty.");
            }
        }

        public void UpdateUrgencyLevel(int ticketId, TicketUrgencyLevelEnum newUrgencyLevel)
        {
            Ticket targetTicket = ticketRepository.GetById(ticketId);
            targetTicket.UpdateUrgencyLevel(newUrgencyLevel);
            ticketRepository.UpdateById(ticketId, targetTicket);
        }

        public void UpdateStatus(int ticketId, TicketStatusEnum newStatus)
        {
            Ticket targetTicket = ticketRepository.GetById(ticketId);
            targetTicket.UpdateStatus(newStatus);
            ticketRepository.UpdateById(ticketId, targetTicket);
        }

        public IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter)
        {
            switch (filter)
            {
                case TicketFilterStatusEnum.OPEN:
                    return tickets.Where(IsStatusOpen);
                case TicketFilterStatusEnum.IN_PROGRESS:
                    return tickets.Where(IsStatusInProgress);
                case TicketFilterStatusEnum.RESOLVED:
                    return tickets.Where(IsStatusResolved);
                default:
                    return tickets;
            }
        }
        private bool IsStatusOpen(TicketDTO ticket) => ticket.currentStatus == TicketStatusEnum.OPEN;
        private bool IsStatusInProgress(TicketDTO ticket) => ticket.currentStatus == TicketStatusEnum.IN_PROGRESS;
        private bool IsStatusResolved(TicketDTO ticket) => ticket.currentStatus == TicketStatusEnum.RESOLVED;
    }
}
