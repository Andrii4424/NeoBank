using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Identity;
using Bank.API.Application.DTOs.Users;
using Bank.API.Application.DTOs.Users.Vacancies;
using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.Entities.Users;
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
            CreateMap<ProfileDto, ApplicationUser>();
            CreateMap<ApplicationUser, CroppedProfileDto>();

            //Bank
            CreateMap<BankEntity, BankDto>();
            CreateMap<BankDto, BankEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.ClientsCount, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveClientsCount, opt => opt.Ignore())
                .ForMember(dest => dest.BlockedClientsCount, opt => opt.Ignore())
                .ForMember(dest => dest.EstablishedDate, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeesCount, opt => opt.Ignore());

            //UserCards
            CreateMap<UserCardsEntity, UserCardsDto>();
            CreateMap<UserCardsDto, UserCardsEntity>();


            //CardTariffs
            CreateMap<CardTariffsEntity, CardTariffsDto>();
            CreateMap<CardTariffsDto, CardTariffsEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            //Vacancies
            CreateMap<VacancyEntity, VacancyDto>();
            CreateMap<VacancyDto, VacancyEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PublicationDate, opt => opt.Ignore());


        }
    }
}
