using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<HomeBankingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion")));

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<HomeBankingContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("HomeBankingConexion")));

builder.Services.AddScoped<IClientRepository, ClientRepository>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();    

var app = builder.Build();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<HomeBankingContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
