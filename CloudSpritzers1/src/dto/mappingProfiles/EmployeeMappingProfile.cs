using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model.Employee;

namespace CloudSpritzers1.Src.Dto.MappingProfiles
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EmployeeDTO>()
                .ConstructUsing(employee => new EmployeeDTO(
                    employee.RetrieveConfiguredDisplayFullNameForBot(),
                    employee.RetrieveConfiguredEmailAddressForBotContact()))
                .ForAllMembers(opt => opt.Ignore());
        }
    }
}
