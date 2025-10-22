using Bank.API.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Domain.RepositoryContracts.Users
{
    public interface IUserRepository
    {
        public Task<Dictionary<Guid, List<string?>>> GetRolesDictionaryAsync(List<ApplicationUser> users);
    }
}
