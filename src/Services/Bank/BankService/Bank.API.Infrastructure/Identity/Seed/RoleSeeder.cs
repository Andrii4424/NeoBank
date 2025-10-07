using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.API.Infrastructure.Identity.Seed
{
    public static class RoleSeeder
    {
        private static readonly string[] roles = { "Admin", "User" };

        public static async Task SeedAsync(IServiceProvider sp, IConfiguration configuration)
        {
            using var scope = sp.CreateScope();
           
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //Roles
            foreach (var r in roles)
            {
                if (!await roleMgr.RoleExistsAsync(r))
                {
                    await roleMgr.CreateAsync(new ApplicationRole { Name = r });
                }
            }

            //SuperAdmin
            var adminEmail = configuration["AdminUser:Email"];
            var adminPassword = configuration["AdminUser:Password"];

            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var create = await userMgr.CreateAsync(admin, adminPassword);
                if (!create.Succeeded) throw new Exception(string.Join("; ", create.Errors.Select(e => e.Description)));
            }

            if (!await userMgr.IsInRoleAsync(admin, "Admin"))
                await userMgr.AddToRoleAsync(admin, "Admin");
        }
    }
}
