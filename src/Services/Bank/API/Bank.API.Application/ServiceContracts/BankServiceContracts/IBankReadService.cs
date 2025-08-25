using Bank.API.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts
{
    public interface IBankReadService
    {
        public Task<BankDto> GetBank();
    }
}
