using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses.Filters.User
{
    public class UserCreditsFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SearchValue { get; set; }

        public string? SortValue { get; set; }

        public List<Currency>? ChosenCurrency { get; set; }

        public bool ShowClosedCredits { get; set; } = false;

        public FiltersDto<UserCreditEntity> ToGeneralFilters(Guid userId)
        {
            Expression<Func<UserCreditEntity, bool>>? searchFilter = String.IsNullOrWhiteSpace(SearchValue) ? null : c => c.CreditTariffs.Name.Contains(SearchValue.Trim());

            Expression<Func<UserCreditEntity, object>>? sortExpression;
            bool ascending;


            switch (SortValue)
            {
                case "date-descending":
                    ascending = false;
                    sortExpression = c => c.CreatedAt;
                    break;
                case "date-ascending":
                    ascending = true;
                    sortExpression = c => c.CreatedAt;
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
                    sortExpression = c => c.CreatedAt;
                    break;
            }

            List<Expression<Func<UserCreditEntity, bool>>>? filters = new();

            if (ChosenCurrency != null)
            {
                filters.Add(c => ChosenCurrency.Contains(c.Currency));
            }
            if (!ShowClosedCredits)
            {
                filters.Add(c => c.IsClosed == false);
            }
            filters.Add(c => c.UserId == userId);

            return new FiltersDto<UserCreditEntity>(PageNumber, PageSize, searchFilter, sortExpression, ascending, filters);

        }
    }
}
