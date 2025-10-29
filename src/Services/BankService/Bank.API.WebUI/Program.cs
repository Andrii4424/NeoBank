using Bank.API.Infrastructure.Identity.Seed;
using Bank.API.WebUI.StartupServicesInjection;
using DotNetEnv;
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

AddApplicationServices.AddServices(builder.Services, builder.Configuration);

//Serilog
builder.Host.UseSerilog((context, config) => {
    config
        .WriteTo.Console() 
        .WriteTo.File("logs/log.txt");
});

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

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();

app.Run();
