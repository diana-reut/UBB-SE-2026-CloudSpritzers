using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model.Ticket;
using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1.Src.Dto.MappingProfiles
{
    public class TicketMappingProfile : Profile
    {
        public TicketMappingProfile()
        {
            CreateMap<Ticket, TicketDTO>()
                .ConstructUsing(ticket => new TicketDTO(
                    ticket.TicketId,
                    ticket.Creator.UserId,
                    ticket.Creator.RetrieveConfiguredEmailAddressForBotContact(),
                    ticket.UrgencyLevel,
                    ticket.CurrentStatus,
                    ticket.Category.CategoryId,
                    ticket.Category.CategoryName,
                    ticket.Subcategory.SubcategoryId,
                    ticket.Subcategory.SubcategoryName,
                    ticket.Subject,
                    ticket.Description,
                    ticket.CreationTimestamp));
        }
    }
}
