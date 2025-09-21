using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Application.Helpers.HelperClasses
{
    public class PageResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageCount { get; set; } 
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool HasNextPage => PageNumber * PageSize < TotalCount;
        public bool HasPreviousPage => PageNumber > 1;


        public PageResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? Enumerable.Empty<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            PageCount = (int)Math.Ceiling(totalCount /10.0);
        }
    }
}
