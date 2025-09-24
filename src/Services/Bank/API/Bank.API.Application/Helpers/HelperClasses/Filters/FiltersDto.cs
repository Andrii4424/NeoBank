using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses.Filters
{
    public class FiltersDto<T> where T : class
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public Expression<Func<T, bool>>? SearchFilter { get; set; }

        public bool Ascending { get; set; }

        public Expression<Func<T, object>>? SortValue { get; set; }

        public List<Expression<Func<T, bool>>>? Filters { get; set; }

        public FiltersDto(int? pageNumber, int? pageSize, Expression<Func<T, bool>>? searchFilter, 
            Expression<Func<T, object>>? sortValue, bool ascending, List<Expression<Func<T, bool>>>? filters)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchFilter = searchFilter;
            SortValue = sortValue;
            Ascending = ascending;
            Filters = filters;
        } 
    }
}
