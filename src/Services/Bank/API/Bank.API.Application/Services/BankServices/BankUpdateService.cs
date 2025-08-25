using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Domain.Entities;
using Bank.API.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Services.BankServices
{
    public class BankUpdateService : IBankUpdateService
    {
        private readonly IBankRepository _bankRepository;
        private readonly IMapper _mapper;

        public BankUpdateService(IBankRepository bankRepository, IMapper mapper)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }

        public async Task<OperationResult> UpdateBank(BankDto bankDto)
        {
            BankEntity bank = _mapper.Map<BankEntity>(bankDto);
            _bankRepository.UpdateObject(bank);
            await _bankRepository.SaveAsync();

            return OperationResult.Ok();
        }
    }
}
