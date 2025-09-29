using AutoMapper;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices.Users
{
    public class UserCardService
    {
        private readonly IUserCardsRepository _userCardsRepository;
        private readonly IMapper _mapper;

        public UserCardService(IUserCardsRepository userCardsRepository, IMapper mapper)
        {
            _userCardsRepository = userCardsRepository;
            _mapper = mapper;
        }


    }
}
