using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.DTOs.Credits;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Domain.Entities.Credits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts.Credits
{
    public interface ICreditTariffsService
    {
        public Task<PageResult<CreditTariffsDto>> GetCreditTariffsPage(CreditTariffsFilter? filtersDto);
        public Task<CreditTariffsDto?> GetCreditAsync(Guid? cardId);
        public Task<OperationResult> AddAsync(CreditTariffsDto creditTariffsDto);
        public Task<OperationResult> UpdateAcync(CreditTariffsDto creditTariffsDto);
        public Task<OperationResult> DeleteAsync(Guid creditId);
    }
}
