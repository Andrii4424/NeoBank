using Bank.API.Application.Helpers.Mapping;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Application.Services.BankServices;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Infrastructure.Data;
using Bank.API.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bank.API.WebUI.StartupServicesInjection
{
    public static class AddApplicationServices
    {
        public static IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddCors();

            //Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //DbContext
            services.AddDbContext<BankAppContext>(
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                }
                );

            //Repositories injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IBankRepository), typeof(BankRepository));

            //Services injection
            services.AddScoped<IBankReadService, BankReadService>();
            services.AddScoped<IBankUpdateService, BankUpdateService>();


            //AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
