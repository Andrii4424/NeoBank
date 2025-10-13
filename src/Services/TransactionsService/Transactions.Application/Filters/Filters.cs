using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Entities;

namespace Transactions.Application.Filters
{
    public class Filters<T> where T : class
    {
        //Pagination
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool? Ascending { get; set; }

        //Expressions
        public Expression<Func<T, object>>? SortExpression { get; set; }
        public List<Expression<Func<T, bool>>>? FiltersExpression { get; set; }

        public Filters(int? pageNumber, int pageSize, bool? ascending, Expression<Func<T, object>>? sortExpression, 
            List<Expression<Func<T, bool>>>? filtersExpression)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Ascending = ascending;
            SortExpression = sortExpression;
            FiltersExpression = filtersExpression;
        }

    }
}
