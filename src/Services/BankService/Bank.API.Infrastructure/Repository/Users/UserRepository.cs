using Bank.API.Domain.Entities.Identity;
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
    public class UserRepository : IUserRepository
    {
        private readonly BankAppContext _context;

        public UserRepository(BankAppContext context) { 
            _context = context;
        }

        public async Task<Dictionary<Guid, List<string?>>> GetRolesDictionaryAsync(List<ApplicationUser> users)
        {
            IEnumerable<Guid> userIds = users.Select(u => u.Id);
            Dictionary<Guid, List<string?>> rolesMap = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId)) 
                .Join(_context.Roles,
                    userRole => userRole.RoleId, 
                    role => role.Id,             
                    (userRole, role) => new { userRole.UserId, role.Name } 
                )
                .GroupBy(x => x.UserId) 
                .ToDictionaryAsync(
                    g => g.Key,                         
                    g => g.Select(x => x.Name).ToList()  
                );
            return rolesMap;
        }
    }
}
