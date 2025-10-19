using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Users;
using Bank.API.Domain.RepositoryContracts.Users;
using Bank.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Repository.Users
{
    public class VacanciesRepository : GenericRepository<VacancyEntity>, IVacanciesRepository
    {
        private readonly DbSet<VacancyEntity> _dbSet;
        public VacanciesRepository(BankAppContext context) : base(context)
        {
            _dbSet = context.Set<VacancyEntity>();
        }
    }
}
