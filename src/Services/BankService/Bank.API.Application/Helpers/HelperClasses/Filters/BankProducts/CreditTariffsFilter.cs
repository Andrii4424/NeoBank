using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses.Filters.BankProducts
{
    public class CreditTariffsFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } =10;

        public string? SearchValue { get; set; }

        public string? SortValue { get; set; }

        public List<Currency>? ChosenCurrency { get; set; }


        public FiltersDto<CreditTariffsEntity> ToGeneralFilters()
        {
            Expression<Func<CreditTariffsEntity, bool>>? searchFilter = String.IsNullOrWhiteSpace(SearchValue) ? null : c => c.Name.Contains(SearchValue.Trim());

            Expression<Func<CreditTariffsEntity, object>>? sortExpression;
            bool ascending;


            switch (SortValue)
            {
                case "name-descending":
                    ascending = false;
                    sortExpression = c => c.Name;
                    break;
                case "name-ascending":
                    ascending = true;
                    sortExpression = c => c.Name;
                    break;
                case "interest-rate-ascending":
                    ascending = false;
                    sortExpression = c => c.InterestRate;
                    break;
                case "interest-rate-descending":
                    ascending = true;
                    sortExpression = c => c.InterestRate;
                    break;
                default:
                    ascending = false;
                    sortExpression = c => c.Name;
                    break;
            }

            List<Expression<Func<CreditTariffsEntity, bool>>>? filters = new();

            if (ChosenCurrency != null)
            {
                filters.Add(c => c.EnableCurrency.Any(currency => ChosenCurrency.Contains(currency)));
            }

            return new FiltersDto<CreditTariffsEntity>(PageNumber, PageSize, searchFilter, sortExpression, ascending, filters);

        }
    }
}
