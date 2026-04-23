using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.viewModel;
using System;
using System.Collections.Generic;

namespace CloudSpritzers1.src.service.interfaces
{
    public interface ITicketService
    {
        void CreateTicket(int ticketId, User ticketCreator, TicketStatusEnum initialStatus,
            TicketCategory category, TicketSubcategory subcategory,
            string subject, string description, DateTime creationTimestamp,
            TicketUrgencyLevelEnum? initialUrgencyLevel = null);

        void AddTicket(Ticket ticketEntity);

        void DeleteTicketById(int ticketId);

        Ticket GetTicketById(int ticketId);

        IEnumerable<Ticket> GetAllTickets();

        void UpdateTicketById(int id, Ticket ticket);

        void UpdateUrgencyLevel(int ticketId, TicketUrgencyLevelEnum newUrgencyLevel);

        void UpdateStatus(int ticketId, TicketStatusEnum newStatus);

        IEnumerable<TicketDTO> FilterTicketsByStatus(IEnumerable<TicketDTO> tickets, TicketFilterStatusEnum filter);
    }
}