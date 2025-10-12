using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Transactions.Domain.Entities;

namespace Transactions.Application.Filters
{
    public class TransactionFilter
    {
        //Pagination
        public int? PageNumber { get; set; }
        public int PageSize { get; set; } = 50;

        //Sorting
        public string? SortValue { get; set; }
        public bool? Ascending { get; set; }

        //Filters
        public DateOnly? ChosenDate { get; set; }
        public double? MinimalTransactionSum { get; set; }

        //Expressions
        public Expression<Func<TransactionEntity, object>>? SortExpression { get; set; }
        public List<Expression<Func<TransactionEntity, bool>>>? Filters { get; set; }

        public TransactionFilter()
        {

        }

        public void ConvertToExpressions()
        {
            switch (SortValue)
            {
                case "date-descending":
                    Ascending = false;
                    SortExpression = t => t.TransactionTime;
                    break;
                case "date-ascending":
                    Ascending = true;
                    SortExpression = t => t.TransactionTime;
                    break;
                case "transaction-sum-descending":
                    Ascending = false;
                    SortExpression = t => t.Amount;
                    break;
                case "transaction-sum-ascending":
                    Ascending = true;
                    SortExpression = t => t.Amount;
                    break;
                default:
                    Ascending = false;
                    SortExpression = t => t.TransactionTime;
                    break;
            }

            Filters = new List<Expression<Func<TransactionEntity, bool>>>();

            if (ChosenDate != null)
            {
                Filters.Add(t => DateOnly.FromDateTime(t.TransactionTime.Value) == ChosenDate);
            }
            if (MinimalTransactionSum != null)
            {
                Filters.Add(t => t.Amount >= (decimal)MinimalTransactionSum);
            }

        }
    }
}
