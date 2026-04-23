using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Repository;

namespace CloudSpritzers1.Src.Dto
{
    public record TicketDTO(
        int ticketId,
        int creatorAccountId,
        string creatorEmailAddress,
        TicketUrgencyLevelEnum urgencyLevel,
        TicketStatusEnum currentStatus,
        int categoryId,
        string categoryName,
        int subcategoryId,
        string subcategoryName,
        string subject,
        string description,
        DateTime creationTimestamp)
    { }
}
