using Bank.API.WebUI.StartupServicesInjection;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

AddApplicationServices.AddServices(builder.Services, builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}
else
{
    app.UseHsts();
}

app.UseStaticFiles();
app.MapControllers();
app.UseRouting();

app.Run();
