using Bank.API.Application.DTOs.BankProducts;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts;
using Bank.API.Domain.Entities.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts
{
    public interface ICardTariffsService
    {
        public Task<PageResult<CardTariffsDto>> GetDefaultPageAsync();
        public Task<PageResult<CardTariffsDto>> GetCardsAsync(CardTariffsFilter filters);
        public Task<OperationResult> AddAsync(CardTariffsDto cardDto);
        public Task<OperationResult> UpdateAcync(CardTariffsDto cardDto);
        public Task<OperationResult> DeleteAsync(Guid cardId);
    }
}
