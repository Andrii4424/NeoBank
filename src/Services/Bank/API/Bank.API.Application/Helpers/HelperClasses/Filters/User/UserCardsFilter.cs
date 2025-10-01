using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses.Filters.User
{
    public class UserCardsFilter
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public string? SearchValue { get; set; }

        public string? SortValue { get; set; }

        public List<PaymentSystem>? ChosenPaymentSystems { get; set; }
        public List<Currency>? ChosenCurrency { get; set; }
        public List<CardLevel>? ChosenLevels { get; set; }
        public List<CardType>? ChosenTypes { get; set; }


        public FiltersDto<UserCardsEntity> ToGeneralFilters()
        {
            Expression<Func<UserCardsEntity, bool>>? searchFilter = String.IsNullOrWhiteSpace(SearchValue) ? null : c => c.CardTariff.CardName.Contains(SearchValue.Trim());

            Expression<Func<UserCardsEntity, object>>? sortExpression;
            bool ascending;


            switch (SortValue)
            {
                case "balance-ascending":
                    ascending = true;
                    sortExpression = c => c.Balance;
                    break;
                case "balance-descending":
                    ascending = false;
                    sortExpression = c => c.Balance;
                    break;
                case "validity-period":
                    ascending = false;
                    sortExpression = c => c.ExpiryDate;
                    break;
                default:
                    ascending = true;
                    sortExpression = c => c.Balance;
                    break;
            }

            List<Expression<Func<UserCardsEntity, bool>>>? filters = new();

            if (ChosenPaymentSystems != null)
            {

                filters.Add(c => ChosenPaymentSystems.Contains(c.ChosedPaymentSystem));
            }
            if (ChosenCurrency != null)
            {
                filters.Add(c => ChosenCurrency.Contains(c.ChosenCurrency));
            }
            if (ChosenLevels != null)
            {
                foreach (var item in ChosenLevels)
                {
                    filters.Add(c => ChosenLevels.Contains(c.CardTariff.Level));
                }
            }
            if (ChosenTypes != null)
            {
                foreach (var item in ChosenTypes)
                {
                    filters.Add(c => ChosenTypes.Contains(c.CardTariff.Type));
                }
            }

            return new FiltersDto<UserCardsEntity>(PageNumber, PageSize, searchFilter, sortExpression, ascending, filters);
        }
    }
}
