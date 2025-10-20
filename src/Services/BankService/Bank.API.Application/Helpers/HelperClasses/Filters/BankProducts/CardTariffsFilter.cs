using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts
{
    public class CardTariffsFilter
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public string? SearchValue { get; set; }

        public string? SortValue { get; set; }

        public List<PaymentSystem>? ChosenPaymentSystems { get; set; }
        public List<Currency>? ChosenCurrency { get; set; }
        public List<CardLevel>? ChosenLevels { get; set; }
        public List<CardType>? ChosenTypes { get; set; }


        public FiltersDto<CardTariffsEntity> ToGeneralFilters()
        {
            Expression<Func<CardTariffsEntity, bool>>? searchFilter = String.IsNullOrWhiteSpace(SearchValue)? null : c => c.CardName.Contains(SearchValue.Trim());

            Expression<Func<CardTariffsEntity, object>>? sortExpression;
            bool ascending;


            switch (SortValue)
            {
                case "name-descending":
                    ascending = false;
                    sortExpression = c => c.CardName;
                    break;
                case "name-ascending":
                    ascending = true;
                    sortExpression = c => c.CardName;
                    break;
                case "annual-maintenance-cost":
                    ascending = true;
                    sortExpression = c => c.AnnualMaintenanceCost;
                    break;
                case "validity-period":
                    ascending = false;
                    sortExpression = c => c.ValidityPeriod;
                    break;
                default:
                    ascending = false;
                    sortExpression = c => c.CardName;
                    break;
            }

            List<Expression<Func<CardTariffsEntity, bool>>>? filters = new();

            if (ChosenPaymentSystems != null)
            {
                filters.Add(c => c.EnabledPaymentSystems.Any(paymentsystem => ChosenPaymentSystems.Contains(paymentsystem)));
            }
            if (ChosenCurrency != null)
            {
                filters.Add(c => c.EnableCurrency.Any(currency => ChosenCurrency.Contains(currency)));
            }
            if (ChosenLevels != null)
            {
                foreach (var item in ChosenLevels)
                {
                    filters.Add(c => ChosenLevels.Contains(c.Level));
                }
            }
            if (ChosenTypes != null)
            {
                foreach (var item in ChosenTypes)
                {
                    filters.Add(c => ChosenTypes.Contains(c.Type));
                }
            }

            return new FiltersDto<CardTariffsEntity>(PageNumber, PageSize, searchFilter, sortExpression, ascending, filters);

        }
    }
}
