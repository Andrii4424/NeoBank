using AutoMapper;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts.Credits;
using Bank.API.Application.Services.BankServices;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.CreditsServices
{
    public class CreditTariffsService : ICreditTariffsService
    {
        private readonly ICreditTariffsRepository _creditTariffsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreditTariffsService> _logger;

        public CreditTariffsService(ICreditTariffsRepository creditTariffsRepository, IMapper mapper, ILogger<CreditTariffsService> logger)
        {
            _creditTariffsRepository = creditTariffsRepository;
            _mapper = mapper;
            _logger = logger;
        }


        //Read Services
    }
}
