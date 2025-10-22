using Bank.API.Domain.Entities.Identity;
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
    public class UserFilter
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; } = 50;

        public string? SearchValue { get; set; }

        public string? SortValue { get; set; }

        public bool? VerifiedUsers { get; set; }
        public bool? WithAvatars { get; set; }
        public bool? HasFinancalNumber { get; set; }



        public FiltersDto<ApplicationUser> ToGeneralFilters()
        {
            Expression<Func<ApplicationUser, bool>>? searchFilter = String.IsNullOrWhiteSpace(SearchValue) ? null : 
                (u => (u.FirstName!=null && u.FirstName.Contains(SearchValue.Trim())) ||(u.Surname !=null && u.Surname.Contains(SearchValue.Trim())) ||
                (u.Patronymic != null && u.Patronymic.Contains(SearchValue.Trim())));

            Expression<Func<ApplicationUser, object>>? sortExpression;
            bool ascending;

            switch (SortValue)
            {
                case "surname-descending":
                    ascending = false;
                    sortExpression = u => u.Surname;
                    break;
                case "surname-ascending":
                    ascending = true;
                    sortExpression = u => u.Surname;
                    break;
                case "date-ascending":
                    ascending = true;
                    sortExpression = u => u.DateOfBirth;
                    break;
                case "date-descending":
                    ascending = false;
                    sortExpression = u => u.DateOfBirth;
                    break;
                default:
                    ascending = false;
                    sortExpression = u => u.Surname;
                    break;
            }

            List<Expression<Func<ApplicationUser, bool>>>? filters = new();

            if (VerifiedUsers != null)
            {
                filters.Add(u => u.IsVerified == true);
            }
            if (WithAvatars != null)
            {
                filters.Add(u => u.AvatarPath != null);
            }
            if (HasFinancalNumber != null)
            {
                filters.Add(u => u.PhoneNumber != null);
            }

            return new FiltersDto<ApplicationUser>(PageNumber, PageSize, searchFilter, sortExpression, ascending, filters);
        }

    }
}
