using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.dto
{
    public record TicketDTO(
        int TicketId, 
        int UserId, 
        string UserEmail, 
        UrgencyLevelEnum UrgencyLevel, 
        StatusEnum Status, 
        int CategoryId, 
        string CategoryName, 
        int SubcategoryId, 
        string SubcategoryName, 
        string Subject, 
        string Description, 
        DateTime CreatedAt) 
    { }
}
