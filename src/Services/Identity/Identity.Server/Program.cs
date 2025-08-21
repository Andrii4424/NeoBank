var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();



app.Run();
