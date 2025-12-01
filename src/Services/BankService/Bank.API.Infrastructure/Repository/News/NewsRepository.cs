using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.News;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.News;
using Bank.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository.News
{
    public class NewsRepository : GenericRepository<NewsEntity>, INewsRepository
    {
        private readonly DbSet<NewsEntity> _dbSet;
        public NewsRepository(BankAppContext context) : base(context) {
            _dbSet = context.Set<NewsEntity>();
        }


        public async Task<bool> IsDuplicateNewsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(n=>n.Id==id);
        }
    }
}
