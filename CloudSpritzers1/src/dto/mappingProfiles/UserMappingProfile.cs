using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;

namespace CloudSpritzers1.Src.Dto.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ConstructUsing(user => new UserDTO(
                    user.RetrieveConfiguredDisplayFullNameForBot(),
                    user.RetrieveConfiguredEmailAddressForBotContact())).ForAllMembers(opt => opt.Ignore());
        }
    }
}
