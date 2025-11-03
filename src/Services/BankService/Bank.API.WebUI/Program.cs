using Bank.API.Infrastructure.Identity.Seed;
using Bank.API.WebUI.StartupServicesInjection;
using DotNetEnv;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var envFile = currentEnv.ToLower() switch
{
    "development" => ".env.dev",
    "staging" => ".env.staging",
    _ => ".env.prod"
};

if (File.Exists(envFile))
{
    Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

AddApplicationServices.AddServices(builder.Services, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}
else
{
    app.UseHsts();
}

//Swagger
//Swagger available in production because its a pet project
app.UseSwagger();
app.UseSwaggerUI();

await RoleSeeder.SeedAsync(app.Services, builder.Configuration);

app.UseRouting();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200",
    "https://localhost:7280", "http://localhost:5226")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();

app.Run();
