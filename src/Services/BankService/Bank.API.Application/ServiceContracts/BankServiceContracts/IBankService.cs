using Bank.API.Application.DTO;
using Bank.API.Application.Helpers.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts
{
    public interface IBankService
    {
        public Task<BankDto> GetBankInfo();
        public Task<OperationResult> UpdateBank(BankDto? bankDto);

    }
}
