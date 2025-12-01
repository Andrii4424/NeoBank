using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.API.Domain.Entities;
using Bank.API.Domain.Entities.Cards;
using Bank.API.Domain.Entities.Credits;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.Entities.News;
using Bank.API.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bank.API.Infrastructure.Data
{
    public class BankAppContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public BankAppContext(DbContextOptions<BankAppContext> options) : base(options) { }

        public DbSet<BankEntity> BankInfo { get; set; }
        public DbSet<CardTariffsEntity> CardTariffs { get; set; }
        public DbSet<UserCardsEntity> UserCards { get; set; }
        public DbSet<VacancyEntity> Vacancies { get; set; }
        public DbSet<CreditTariffsEntity> CreditTariffs { get; set; }
        public DbSet<NewsEntity> News { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BankEntity>().ToTable("BankInfo");
            modelBuilder.Entity<CardTariffsEntity>().ToTable("CardTariffs");
            modelBuilder.Entity<VacancyEntity>().ToTable("Vacancies");
            modelBuilder.Entity<CreditTariffsEntity>().ToTable("CreditTariffs");
            modelBuilder.Entity<NewsEntity>().ToTable("News");

            modelBuilder.Entity<CardTariffsEntity>()
                .HasOne(c => c.Bank)
                .WithMany(b => b.Cards)
                .HasForeignKey(c => c.BankId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserCardsEntity>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCards)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserCardsEntity>()
                .HasOne(u => u.CardTariff)
                .WithMany(c => c.UserCards)
                .HasForeignKey(u => u.CardTariffId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<VacancyEntity>()
                .HasOne(c => c.Bank)
                .WithMany(b => b.Vacancies)
                .HasForeignKey(c => c.BankId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}