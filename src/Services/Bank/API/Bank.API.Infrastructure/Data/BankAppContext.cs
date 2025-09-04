using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bank.API.Infrastructure.Data
{
    public class BankAppContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public BankAppContext(DbContextOptions<BankAppContext> options) : base(options) { }

        public DbSet<BankEntity> BankInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BankEntity>().ToTable("BankInfo");
        }
    }
}