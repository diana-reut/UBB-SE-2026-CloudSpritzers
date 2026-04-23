using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Tar;
using AutoMapper;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model.Faq;

namespace CloudSpritzers1.Src.Dto.MappingProfiles
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
                    src.NotHelpfulVotesCount));

            CreateMap<FAQEntryDTO, FAQEntry>()
                .ConstructUsing(src => new FAQEntry(
                    src.Id,
                    src.Question,
                    src.Answer,
                    src.Category,
                    src.ViewCount,
                    src.HelpfulVotesCount,
                    src.NotHelpfulVotesCount));
    }
    }
}
