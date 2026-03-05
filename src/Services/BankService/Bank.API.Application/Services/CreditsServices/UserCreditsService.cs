using AutoMapper;
using Bank.API.Application.DTOs.Credits;
using Bank.API.Application.DTOs.Users.CardOperations;
using Bank.API.Application.DTOs.Users.Cards;
using Bank.API.Application.Helpers.HelperClasses;
using Bank.API.Application.Helpers.HelperClasses.Filters;
using Bank.API.Application.Helpers.HelperClasses.Filters.User;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts.Credits;
using Bank.API.Application.ServiceContracts.BankServiceContracts.Users;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.Enums.CreditEnums;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Domain.RepositoryContracts.Users;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Bank.API.Application.Services.CreditsServices
{
    public class UserCreditsService: IUserCreditsService
    {
        private readonly IUserCreditsRepository _userCreditsRepository;
        private readonly ICreditTariffsRepository _creditTariffsRepository;
        private readonly IUserCardService _userCardService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserCreditsService> _logger;

        public UserCreditsService(IUserCreditsRepository userCreditsRepository, IMapper mapper, ILogger<UserCreditsService> logger, 
            ICreditTariffsRepository creditTariffsRepository, IUserCardService userCardService)
        {
            _userCreditsRepository = userCreditsRepository;
            _mapper = mapper;
            _logger = logger;
            _creditTariffsRepository = creditTariffsRepository;
            _userCardService = userCardService;
        }


        public async Task<PageResult<UserCreditDto>> GetUserCredits(UserCreditsFilter filtersDto, Guid userId)
        {
            _logger.LogDebug("Getting credits for user with ID: {UserId}", userId);

            FiltersDto<UserCreditEntity> filters = filtersDto.ToGeneralFilters(userId);

            List<UserCreditEntity> credits = await _userCreditsRepository.GetFilteredCardsAsync(filters.PageNumber.Value, filters.PageSize.Value, filters.SearchFilter,
                filters.Ascending, filters.SortValue, filters.Filters, true);

            PageResult<UserCreditDto> pageResult = new PageResult<UserCreditDto>(_mapper.Map<List<UserCreditDto>>(credits),
                    await _userCreditsRepository.GetCountAsync(filters.SearchFilter, filters.Filters), filters.PageNumber.Value, filtersDto.PageSize);

            return pageResult;
        }

        public async Task<TransactionDto> OpenUserCredit(TransactionDto transaction)
        {
            _logger.LogDebug("Opening credit for user with ID: {UserId}", transaction.GetterId);

            if (transaction.CreditTariffsId == null || transaction.GetterId == null || transaction.GetterCardId == null 
                || transaction.AmountToReplenish == null || transaction.TermMonths ==null)
            {
                transaction.Success = false;
                return transaction;
            }

            CreditTariffsEntity? creditTariffs = await _creditTariffsRepository.GetValueByIdAsync(transaction.CreditTariffsId.Value);

            if (creditTariffs == null)
            {
                transaction.Success = false;
                return transaction;
            }

            UserCreditEntity credit = new UserCreditEntity();
            
            credit.CreditTariffId = creditTariffs.Id;
            credit.InterestRate = creditTariffs.InterestRate;
            credit.Status = CreditStatus.Active;
            credit.RemainingDebt = (decimal)transaction.AmountToReplenish;
            credit.CurrentMonthAmountDue = (decimal)transaction.AmountToReplenish / transaction.TermMonths.Value;
            credit.CurrentPaymentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            credit.IsClosed = false;
            credit.UserId = transaction.GetterId.Value;
            credit.Amount = (decimal)transaction.AmountToReplenish;
            credit.RemainingDebt = (decimal)transaction.AmountToReplenish;
            credit.StartDate = DateTime.UtcNow;
            credit.EndDate = DateTime.UtcNow.AddMonths(transaction.TermMonths.Value);
            credit.Currency = transaction.TransactionCurrency.Value;
            credit.CreatedAt = DateTime.UtcNow;

            OperationResult transactionResult = await _userCardService.UpdateCardBalanceAsync(transaction.GetterCardId.Value, (decimal)transaction.AmountToReplenish);
            if (!transactionResult.Success)
            {
                transaction.Success = false;
                return transaction;
            }
            else
            {
                await _userCreditsRepository.AddAsync(credit);
                await _userCreditsRepository.SaveAsync();

                transaction.Success = true;
                return transaction;
            }
        }


        public async Task<TransactionDto> PayForCredit(TransactionDto transaction)
        {
            if (transaction == null)
            {
                _logger.LogError("Error update balance for cards. Error with sending or getting transaction info: transaction is null");
                throw new ArgumentNullException("Error with sending or getting transaction info: transaction is null");
            }
            if (transaction.SenderCardId == null || transaction.AmountToWithdrawn == null || transaction.UserCreditId == null)
            {
                transaction.Success = false;
                return transaction;
            }

            UserCardsDto? card = await _userCardService.GetCardByIdAsync(transaction.SenderCardId.Value);
            UserCreditEntity? credit = await _userCreditsRepository.GetValueByIdAsync(transaction.UserCreditId.Value);
            decimal PositiveAmountToWithdrawn = (decimal)transaction.AmountToWithdrawn * -1;

            if (card == null || credit == null) {
                transaction.Success = false;
                return transaction;
            }

            if(card.Balance < PositiveAmountToWithdrawn)
            {
                transaction.Success = false;
                return transaction;
            }

            if (credit.RemainingDebt != 0)
            {
                credit.CurrentMonthAmountDue = credit.CurrentMonthAmountDue < PositiveAmountToWithdrawn ? 0 : credit.CurrentMonthAmountDue - PositiveAmountToWithdrawn;
                credit.RemainingDebt = credit.RemainingDebt < PositiveAmountToWithdrawn ? 0 : credit.RemainingDebt - PositiveAmountToWithdrawn;

                transaction.AmountToWithdrawn = (double)Math.Min(credit.RemainingDebt, PositiveAmountToWithdrawn);

                OperationResult transactionResult =
                    await _userCardService.UpdateCardBalanceAsync(transaction.SenderCardId!.Value, (decimal)transaction.AmountToWithdrawn * -1);

                if (!transactionResult.Success)
                {
                    transaction.Success = false;
                    return transaction;
                }

                if (credit.RemainingDebt == 0) {
                    credit.Status = CreditStatus.Closed;
                    credit.CloseTime = DateTime.UtcNow;
                }
                else if (credit.CurrentMonthAmountDue == 0)
                {
                    credit.CurrentMonthAmountDue = (decimal)credit.RemainingDebt / (credit.TermMonths == 1? 1 : (credit.TermMonths) - 1);
                }

                _userCreditsRepository.UpdateObject(credit);

                await _userCreditsRepository.SaveAsync();
            }

            transaction.Success = true;
            return transaction;
        }
    }
}
