using AutoMapper;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model.faq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Tar;

namespace CloudSpritzers1.src.dto.mappingProfiles
{
    public class FAQEntryMappingProfile : Profile
    {
        public FAQEntryMappingProfile()
        {
            CreateMap<FAQEntry, FAQEntryDTO>()
                .ConstructUsing(src => new FAQEntryDTO(
                    src.Id,
                    src.Question,
                    src.Answer,
                    src.Category,
                    src.ViewCount,
                    src.HelpfulVotesCount,
                    src.NotHelpfulVotesCount
                ));

            CreateMap<FAQEntryDTO, FAQEntry>()
                .ConstructUsing(src => new FAQEntry(
                    src.Id,
                    src.Question,
                    src.Answer,
                    src.Category,
                    src.ViewCount,
                    src.HelpfulVotesCount,
                    src.NotHelpfulVotesCount
                ));
        
    }
    }
}
