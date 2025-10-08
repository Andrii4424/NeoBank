using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transactions.Infrastructure.Data
{
    public class TransactionContextFactory :IDesignTimeDbContextFactory<TransactionContext>
    {
        public TransactionContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables();

            var webUiPath = Path.Combine(basePath, "..", "..", "Bank.API.WebUI");
            if (Directory.Exists(webUiPath))
            {
                builder
                    .AddJsonFile(Path.Combine(webUiPath, "appsettings.json"), optional: true)
                    .AddJsonFile(Path.Combine(webUiPath, "appsettings.Development.json"), optional: true);
            }

            var config = builder.Build();
            var cs = config.GetConnectionString("DefaultConnection")
                     ?? "Server=.;Database=BankDb;Trusted_Connection=True;TrustServerCertificate=True";

            var options = new DbContextOptionsBuilder<TransactionContext>()
                .UseSqlServer(cs)
                .Options;

            return new TransactionContext(options);
        }
    }
}
