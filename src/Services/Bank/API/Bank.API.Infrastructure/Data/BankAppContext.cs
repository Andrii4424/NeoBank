using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bank.API.Infrastructure.Data
{
    public class BankAppContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public BankAppContext(DbContextOptions<BankAppContext> options) : base(options) { }

        public DbSet<BankEntity> BankInfo { get; set; }
        public DbSet<CardTariffsEntity> CardTariffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BankEntity>().ToTable("BankInfo");
            modelBuilder.Entity<CardTariffsEntity>().ToTable("CardTariffs");

            modelBuilder.Entity<CardTariffsEntity>()
                .HasOne(c => c.Bank)
                .WithMany(b => b.Cards)
                .HasForeignKey(c => c.BankId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}