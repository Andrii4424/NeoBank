using Microsoft.EntityFrameworkCore;
using Transactions.Infrastructure.Data;

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

            return services;
        }
    }
}
