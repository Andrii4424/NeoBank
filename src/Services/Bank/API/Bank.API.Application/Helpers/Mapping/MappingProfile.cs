using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.DTOs.Identity;
using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bank.API.Application.Helpers.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile() {
            //User profile
            CreateMap<ApplicationUser, ProfileDto>();

            //Bank
            CreateMap<BankEntity, BankDto>();
            CreateMap<BankDto, BankEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore());

        }

    }
}
