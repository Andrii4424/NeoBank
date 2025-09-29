using AutoMapper;
using Bank.API.Application.DTO;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Domain.Entities;
using Bank.API.Domain.RepositoryContracts;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly string currencyCacheKey = "CurrencyData";
        private readonly IMemoryCache _memoryCache;

        public BankService(IBankRepository bankRepository, IMapper mapper, IMemoryCache memoryCache) {
            _bankRepository = bankRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<BankDto> GetBankInfo()
        {
            return _mapper.Map<BankDto>(await _bankRepository.GetValueByIdAsync(SharedMethods.GetBankGuid()));
        }


        public async Task<OperationResult> UpdateBank(BankDto bankDto)
        {
            BankEntity bank = await _bankRepository.GetValueByIdAsync(bankDto.Id);

           _mapper.Map(bankDto, bank);
            bank.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _bankRepository.UpdateObject(bank);
            await _bankRepository.SaveAsync();
            _memoryCache.Remove(currencyCacheKey);

            return OperationResult.Ok();
        }
    }
}
