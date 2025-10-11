using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Transactions.Application.Helpers;
using Transactions.Application.ServiceContracts;
using Transactions.Application.Services;
using Transactions.Application.Services.MessageServices;
using Transactions.Domain.RepositoryContracts;
using Transactions.Infrastructure.Data;
using Transactions.Infrastructure.Repository;

namespace Transactions.WebUI.Helpers
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
        {
            var culture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

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


            services.AddHttpClient("BankApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7280/api/");
            });

            //DI
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(ITransactionRepository), typeof(TransactionRepository));

            services.AddScoped<ITransactionService, TransactionService>();
            services.AddSingleton<IRabbitMqProducerService, RabbitMqProducerService>();


            //Background Services
            services.AddHostedService<RabbitMqConsumerService>();

            //AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
