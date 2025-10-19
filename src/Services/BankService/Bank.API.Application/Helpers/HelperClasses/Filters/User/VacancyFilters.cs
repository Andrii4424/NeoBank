using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Users;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses.Filters.User
{
    public class VacancyFilters
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; } = 10;

        public string? SearchValue { get; set; }

        public string? SortValue { get; set; }

        public int? minSalary { get; set; }
        public int? maxSalary { get; set; }

        public FiltersDto<VacancyEntity> ToGeneralFilters()
        {
            Expression<Func<VacancyEntity, bool>>? searchFilter = String.IsNullOrWhiteSpace(SearchValue) ? null : v => v.JobTitle.Contains(SearchValue.Trim());

            Expression<Func<VacancyEntity, object>>? sortExpression;
            bool ascending;

            switch (SortValue)
            {
                case "salary-ascending":
                    ascending = true;
                    sortExpression = v => v.Salary;
                    break;
                case "salary-descending":
                    ascending = false;
                    sortExpression = v => v.Salary;
                    break;
                case "date-ascending":
                    ascending = true;
                    sortExpression = v => v.PublicationDate;
                    break;
                case "date-descending":
                    ascending = false;
                    sortExpression = v => v.PublicationDate;
                    break;
                default:
                    ascending = false;
                    sortExpression = v => v.PublicationDate;
                    break;
            }

            List<Expression<Func<VacancyEntity, bool>>>? filters = new();

            if (minSalary != null)
            {
                filters.Add(v => v.Salary>=minSalary);
            }
            if (maxSalary != null) {
                filters.Add(v => v.Salary<=maxSalary);

            }

            return new FiltersDto<VacancyEntity>(PageNumber, PageSize, searchFilter, sortExpression, ascending, filters);
        }
    }
}
