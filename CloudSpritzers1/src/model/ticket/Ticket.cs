using System;

namespace CloudSpritzers.src.model.ticket
 {
    public class Ticket
    {
        public int TicketId { get; }
        public User User { get; }
        public UrgencyLevelEnum UrgencyLevel { get; private set; }
        public StatusEnum Status { get; private set; }
        public TicketCategory Category { get; }
        public TicketSubcategory Subcategory { get; }
        public string Subject { get; }
        public string Description { get; }
        public DateTime CreatedAt { get; }
        public Ticket(int ticketId, User user, StatusEnum status, TicketCategory category, TicketSubcategory subcategory, string subject, string description, DateTime createdAt)
        { 
            TicketId = ticketId;
            User = user;
            UrgencyLevel = category.UrgencyLevel;
            Status = status;
            Category = category;
            Subcategory = subcategory;
            Subject = subject;
            Description = description;
            CreatedAt = createdAt;
        }
        public void UpdateStatus(StatusEnum newStatus)
        {
            this.Status = newStatus;
        }

        public void ChangeUrgencyLevel(UrgencyLevelEnum urgencyLevel)
        {
            this.UrgencyLevel = urgencyLevel;
        }
    }
 }

