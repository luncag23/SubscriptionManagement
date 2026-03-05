using BusinessLogic.AbstractFactory;
using BusinessLogic.Factories;
using DAL.Abstract;
using DAL.Concrete;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// 1. Configurarea Entity Framework Core cu SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SubscriptionDbContext>(options =>
	options.UseSqlServer(connectionString));

// 2. Înregistrarea Repository-ului (Dependency Injection)
// În Program.cs adaugă:
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();


// 3. Tot aici vom înregistra și Service-ul de Business mai târziu
// builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
// 3. Înregistrează Provider-ul (Aceasta este linia care rezolvă eroarea ta!)
builder.Services.AddScoped<SubscriptionProvider>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddScoped<BillingProvider>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
	context.Database.EnsureCreated();

	// 1. Verificăm/Adăugăm Planul FREE
	if (!context.SubscriptionPlans.Any(p => p.Name == "Free Trial Plan"))
	{
		context.SubscriptionPlans.Add(new Domain.Entities.SubscriptionPlan
		{
			Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
			Name = "Free Trial Plan",
			Description = "Acces gratuit pentru 7 zile",
			IsActive = true,
			MonthlyPrice = 0,
			MaxStorageGB = 5
		});
	}

	// 2. Verificăm/Adăugăm Planul PREMIUM (Adobe style)
	if (!context.SubscriptionPlans.Any(p => p.Name == "Premium Plan"))
	{
		context.SubscriptionPlans.Add(new Domain.Entities.SubscriptionPlan
		{
			Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
			Name = "Premium Plan",
			Description = "Acces complet la toate aplicațiile (All Apps Bundle)",
			IsActive = true,
			MonthlyPrice = 50.00m, // Preț de 50 RON/USD
			MaxStorageGB = 100
		});
	}

	// 3. Verificăm/Adăugăm Utilizatorul de test (dacă nu există)
	if (!context.Users.Any(u => u.Email == "test@user.com"))
	{
		context.Users.Add(new Domain.Entities.User
		{
			Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
			Email = "test@user.com",
			FullName = "Test User",
			JoinedDate = DateTime.Now
		});
	}

	context.SaveChanges();
}

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