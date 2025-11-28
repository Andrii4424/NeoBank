using Bank.API.Domain.Entities.News;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts.News
{
    public interface INewsRepository: IGenericRepository<NewsEntity>
    {
        public Task<bool> IsDuplicateNewsAsync(Guid id);
    }
}
