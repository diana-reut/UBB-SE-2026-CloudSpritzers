using CloudSpritzers1.src.model;
using System;

namespace CloudSpritzers1.src.model.ticket
{
    public class Ticket
    {
        public int TicketId { get; }
        public User Creator { get; }
        public TicketUrgencyLevelEnum UrgencyLevel { get; private set; }
        public TicketStatusEnum CurrentStatus { get; private set; }
        public TicketCategory Category { get; }
        public TicketSubcategory Subcategory { get; }
        public string Subject { get; }
        public string Description { get; }
        public DateTime CreationTimestamp { get; }
        public Ticket(int ticketId, User ticketCreator, TicketStatusEnum initialStatus, TicketCategory category, TicketSubcategory subcategory, string ticketSubject, string description, DateTime creationTimestamp, TicketUrgencyLevelEnum? initialUrgencyLevel = null)
        { 
            TicketId = ticketId;
            Creator = ticketCreator;
            UrgencyLevel = initialUrgencyLevel ?? category.CategoryUrgencyLevel;
            CurrentStatus = initialStatus;
            Category = category;
            Subcategory = subcategory;
            Subject = ticketSubject;
            Description = description;
            CreationTimestamp = creationTimestamp;
        }
        public void UpdateStatus(TicketStatusEnum newStatus)
        {
            this.CurrentStatus = newStatus;
        }

        public void UpdateUrgencyLevel(TicketUrgencyLevelEnum newUrgencyLevel)
        {
            this.UrgencyLevel = newUrgencyLevel;
        }
    }
 }

