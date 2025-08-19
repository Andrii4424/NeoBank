using Bank.Identity.WebUI.StartupServicesInjection;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

//Localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-GB"),
        new CultureInfo("uk-UA")
    };
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
    options.DefaultRequestCulture = new RequestCulture("en-GB");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

//Add Services
AddApplicationServices.AddServices(builder.Services, builder.Configuration);

var app = builder.Build();
app.UseRequestLocalization();


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
app.UseRouting();
app.MapControllers();


app.Run();
