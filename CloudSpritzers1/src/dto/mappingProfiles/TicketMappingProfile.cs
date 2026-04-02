using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.dto.mappingProfiles
{
    public class TicketMappingProfile : Profile
    {
        public TicketMappingProfile()
        {
            CreateMap<Ticket, TicketDTO>()
                .ConstructUsing(ticket => new TicketDTO(
                    ticket.TicketId,
                    ticket.User.UserId,
                    ticket.User.GetEmail(),
                    ticket.UrgencyLevel,
                    ticket.Status,
                    ticket.Category.CategoryId,
                    ticket.Category.Name,
                    ticket.Subcategory.SubcategoryId,
                    ticket.Subcategory.SubcategoryName,
                    ticket.Subject,
                    ticket.Description,
                    ticket.CreatedAt
                    ));
        }
    }
}
