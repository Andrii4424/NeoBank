using Bank.API.Application.Helpers.Mapping;
using Bank.API.Application.ServiceContracts;
using Bank.API.Application.ServiceContracts.BankServiceContracts;
using Bank.API.Application.ServiceContracts.BankServiceContracts.BankProducts;
using Bank.API.Application.Services;
using Bank.API.Application.Services.BankServices;
using Bank.API.Application.Services.BankServices.BankProducts;
using Bank.API.Domain.Entities.Identity;
using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.BankProducts;
using Bank.API.Domain.RepositoryContracts.Users;
using Bank.API.Infrastructure.Data;
using Bank.API.Infrastructure.Repository;
using Bank.API.Infrastructure.Repository.BankProducts;
using Bank.API.Infrastructure.Repository.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace Bank.API.WebUI.StartupServicesInjection
{
    public static class AddApplicationServices
    {
        public static IServiceCollection AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });
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

            //Jwt verification
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["AccessToken:Issuer"],
                    ValidAudience = configuration["AccessToken:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["AccessToken:Key"]!)),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
            services.AddAuthorization(options => {});

            //Identity
            services.AddIdentityCore<ApplicationUser>(options =>{
                options.Password.RequireDigit = false;             
                options.Password.RequiredLength = 8;              
                options.Password.RequireNonAlphanumeric = false;    
                options.Password.RequireUppercase = false;          
                options.Password.RequireLowercase = true;          
                options.Password.RequiredUniqueChars = 0;

            })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<BankAppContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminsOnly", p => p.RequireRole("Admin"))
                .AddPolicy("AdminOrUser", p => p.RequireRole("Admin", "User"));

            //Repositories injection
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IBankRepository), typeof(BankRepository));
            services.AddScoped(typeof(ICardTariffsRepository), typeof(CardTariffsRepository));
            services.AddScoped(typeof(IUserCardsRepository), typeof(UserCardsRepository));

            //Services injection
            //Identity
            services.AddScoped<IIdentityService, IdentityService>();
            //Bank and bank products
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<ICardTariffsService, CardTariffsService>();

            //AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
