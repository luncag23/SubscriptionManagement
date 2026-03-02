using DAL.Abstract;
using DAL.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurarea Entity Framework Core cu SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SubscriptionDbContext>(options =>
	options.UseSqlServer(connectionString));

// 2. Înregistrarea Repository-ului (Dependency Injection)
// Folosim Scoped: o instanță nouă pentru fiecare cerere HTTP (recomandat pt DB)
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

// 3. Tot aici vom înregistra și Service-ul de Business mai târziu
// builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Restul codului standard (Middleware, Routing, etc.)
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();