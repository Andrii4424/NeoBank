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
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;
        private readonly IMapper _mapper;

        public BankService(IBankRepository bankRepository, IMapper mapper) {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }

        public async Task<BankDto> GetBankInfo()
        {
            return _mapper.Map<BankDto>(await _bankRepository.GetValueByIdAsync(SharedMethods.GetBankGuid()));
        }


        public async Task<OperationResult> UpdateBank(BankDto bankDto)
        {
            BankEntity bank = await _bankRepository.GetValueByIdAsync(bankDto.Id);

           _mapper.Map(bankDto, bank);
            bank.UpdatedAt = DateTime.UtcNow;
            _bankRepository.UpdateObject(bank);
            await _bankRepository.SaveAsync();

            return OperationResult.Ok();
        }
    }
}
