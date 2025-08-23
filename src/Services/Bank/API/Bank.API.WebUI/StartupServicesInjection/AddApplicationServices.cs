namespace Bank.API.WebUI.StartupServicesInjection
{
    public static class AddApplicationServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();


            return services;
        }
    }
}
