

using Identity.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OpenIddict.Abstractions;
using System.Globalization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Server
{
    //Seeder
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            await RegisterApplicationsAsync(scope.ServiceProvider);
            await RegisterScopesAsync(scope.ServiceProvider);

            //Creating users
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Admin
            var adminEmail = "admin@example.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "123456");
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Failed to create admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            if (!await userManager.IsInRoleAsync(admin, "Admin"))
                await userManager.AddToRoleAsync(admin, "Admin");
            if (!await userManager.IsInRoleAsync(admin, "User"))
                await userManager.AddToRoleAsync(admin, "User");

            // Regular user
            var userEmail = "user@example.com";
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "123456");
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            if (!await userManager.IsInRoleAsync(user, "User"))
                await userManager.AddToRoleAsync(user, "User");

        }

        private async Task RegisterApplicationsAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();

            // API
            if (await manager.FindByClientIdAsync(Configuration["Authentication:Introspection:ClientId"]) == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = Configuration["Authentication:Introspection:ClientId"],
                    ClientSecret = Configuration["Authentication:Introspection:ClientSecret"],
                    Permissions =
                    {
                        Permissions.Endpoints.Introspection
                    }
                };

                await manager.CreateAsync(descriptor);
            }

            // Blazor Hosted
            if (await manager.FindByClientIdAsync(Configuration["Authentication:Blazor:ClientId"]) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = Configuration["Authentication:Blazor:ClientId"],
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = "Blazor code PKCE",
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:44348/callback/logout/local")
                    },
                    RedirectUris =
                    {
                        new Uri("https://localhost:44348/callback/login/local")
                    },
                    ClientSecret = Configuration["Authentication:Blazor:ClientSecret"],
                    Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.EndSession,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,
                        Permissions.Prefixes.Scope + "neobank_api1"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
        }

        private async Task RegisterScopesAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

            if (await manager.FindByNameAsync("neobank_api1") is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = "NeoBank API access",
                    DisplayNames =
                    {
                        [CultureInfo.GetCultureInfo("uk-UA")] = "Доступ до NeoBank  API"
                    },
                    Name = "neobank_api1",
                    Resources =
                    {
                        "neobank_api"
                    }
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
