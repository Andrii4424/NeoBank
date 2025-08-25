using Bank.API.Application.Helpers.Mapping;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Application.Services.BankServices;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

            //Repositories injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(IGenericRepository<>));
            services.AddScoped(typeof(IBankRepository), typeof(IBankRepository));

            //Services injection
            services.AddScoped<IBankReadService, BankReadService>();
            services.AddScoped<IBankUpdateService, BankUpdateService>();


            //AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
