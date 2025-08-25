using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices
{
    public class BankReadService : IBankReadService
    {
        private readonly IBankRepository _bankRepository;
        private readonly IMapper _mapper;

        public BankReadService(IBankRepository bankRepository, IMapper mapper) {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }

        public async Task<BankDto> GetBank()
        {
            return _mapper.Map<BankDto>(await _bankRepository.GetValueByIdAsync(SharedMethods.GetBankGuid()));
        }
    }
}
