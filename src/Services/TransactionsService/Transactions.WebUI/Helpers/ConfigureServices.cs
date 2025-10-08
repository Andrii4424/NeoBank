using Microsoft.EntityFrameworkCore;
using Transactions.Domain.RepositoryContracts;
using Transactions.Infrastructure.Data;
using Transactions.Infrastructure.Repository;

namespace Transactions.WebUI.Helpers
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddCors();

            //Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //DbContext
            services.AddDbContext<TransactionContext>(
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                }
                );

            //DI
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(ITransactionRepository), typeof(TransactionRepository));


            return services;
        }
    }
}
