using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.News;
using Bank.API.Domain.Enums.CardEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses.Filters.News
{
    public class NewsFilter
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; } = 6;

        public FiltersDto<NewsEntity> ToGeneralFilters()
        {

            Expression<Func<NewsEntity, object>>? sortExpression;
            bool ascending;


            ascending = false;
            sortExpression = n => n.CreatedAt;

            return new FiltersDto<NewsEntity>(PageNumber, PageSize, null, sortExpression, ascending, null);

        }
    }
}
