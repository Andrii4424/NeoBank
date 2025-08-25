using Bank.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bank.API.WebUI.StartupServicesInjection
{
    public static class AddApplicationServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            //DbContext
            services.AddDbContext<BankAppContext>(
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                }
                );



            return services;
        }
    }
}
