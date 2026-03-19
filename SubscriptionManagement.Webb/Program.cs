using BusinessLogic.AbstractFactory;
using BusinessLogic.Adapters;
using BusinessLogic.Builder;
using BusinessLogic.Facade;
using BusinessLogic.Factories;
using DAL.Abstract;
using DAL.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. PERSISTENȚĂ (Baza de Date) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SubscriptionDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

// --- 2. SERVICII ȘI PATTERN-URI (Business Logic) ---

// Pattern: Factory Method
builder.Services.AddScoped<SubscriptionProvider>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

// Pattern: Abstract Factory
builder.Services.AddScoped<BillingProvider>();

// Pattern: Builder
builder.Services.AddScoped<IUserProfileBuilder, UserProfileBuilder>();
builder.Services.AddScoped<ProfileDirector>();

// Această metodă instanțiază automat HttpClient și îl dă adaptorului
builder.Services.AddHttpClient<ICurrencyConverter, CurrencyAdapter>();

//Facade
builder.Services.AddScoped<SubscriptionPurchaseFacade>();

// --- 3. SECURITATE (Autentificare) ---
builder.Services.AddAuthentication("CookieAuth")
	.AddCookie("CookieAuth", config => {
		config.Cookie.Name = "User.Auth";
		config.LoginPath = "/Account/Login"; // Unde trimitem userul dacă nu e logat
		config.AccessDeniedPath = "/Home/AccessDenied";
	});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- 4. SEEDING DINAMIC (Populare DB la pornire) ---
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
	// context.Database.Migrate(); // Opțional: rulează migrările automat
	context.Database.EnsureCreated();

	// Adăugare Planuri
	if (!context.SubscriptionPlans.Any())
	{
		context.SubscriptionPlans.AddRange(
			new Domain.Entities.SubscriptionPlan
			{
				Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
				Name = "Free Trial Plan",
				Description = "Acces gratuit pentru 7 zile",
				IsActive = true,
				MonthlyPrice = 0,
				MaxStorageGB = 20
			},
			new Domain.Entities.SubscriptionPlan
			{
				Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
				Name = "Premium Plan",
				Description = "Acces complet la toate aplicațiile (All Apps Bundle)",
				IsActive = true,
				MonthlyPrice = 1000.00m,
				MaxStorageGB = 100
			}
		);
	}

	// Utilizator de test (opțional, acum că avem Register)
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

// --- 5. MIDDLEWARE PIPELINE (Ordinea este critică!) ---
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// !!! IMPORTANT: Autentificarea trebuie să fie ÎNAINTE de Autorizare !!!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();