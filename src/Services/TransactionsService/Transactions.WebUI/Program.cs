using Transactions.WebUI.Helpers;

var builder = WebApplication.CreateBuilder(args);



ConfigureServices.AddServices(builder.Services, builder.Configuration);

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

app.UseRouting();

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200", 
    "http://localhost:5226")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());

app.UseStaticFiles();
app.MapControllers();

app.Run();
