using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Domain.Entities;
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
            //Bank
            CreateMap<BankEntity, BankDto>();
            CreateMap<BankDto, BankEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore());

        }

    }
}
