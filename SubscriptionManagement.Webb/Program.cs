using BusinessLogic.AbstractFactory;
using BusinessLogic.Adapters;
using BusinessLogic.Builder;
using BusinessLogic.Composite;
using BusinessLogic.Facade;
using BusinessLogic.Factories;
using BusinessLogic.Proxy;
using BusinessLogic.Security;
using BusinessLogic.Strategies;
using DAL.Abstract;
using DAL.Concrete;
using Domain.Entities;
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

//Composite
builder.Services.AddScoped<CreativeToolAssembly>();

// Proxy
// 1. Necesare pentru a citi userul curent în Proxy
builder.Services.AddHttpContextAccessor();

// 2. Înregistrarea pentru Admin Proxy
builder.Services.AddScoped<AdminService>(); // Obiectul real
builder.Services.AddScoped<IAdminService, AdminProxy>(); // Proxy-ul (care cere AdminService în constructor)

// 3. Înregistrarea pentru Subscription Proxy
builder.Services.AddScoped<SubscriptionService>(); // Obiectul real
builder.Services.AddScoped<ISubscriptionService, SubscriptionProxy>(); // Proxy-ul

// --- 3. SECURITATE (Autentificare) ---
// În Program.cs, modifică secțiunea de Autentificare:
builder.Services.AddAuthentication(options => {
	options.DefaultScheme = "CookieAuth";
	options.DefaultChallengeScheme = "Google"; // Dacă nu e logat, poate cere Google
})
.AddCookie("CookieAuth", config => {
	config.Cookie.Name = "User.Auth";
	config.LoginPath = "/Account/Login";
})
.AddGoogle(options => {
	options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
	options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
	options.SignInScheme = "CookieAuth";
});

builder.Services.AddControllersWithViews();

// ... în secțiunea de pattern-uri ...
builder.Services.AddScoped<AuthProvider>();

var app = builder.Build();

// --- 4. SEEDING DINAMIC (Un singur bloc pentru tot) ---
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();
	// Ne asigurăm că baza de date este creată conform noii structuri (DbContext-ul tău actualizat)
	context.Database.EnsureCreated();

	// A. SEED PLANURI DE ACCES (Regulile de preț - Prototype)
	// Folosim noile coloane: PlanTypeCode și PriceMultiplier
	if (!context.SubscriptionPlans.Any())
	{
		context.SubscriptionPlans.AddRange(
		    new SubscriptionPlan
		    {
			    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
			    Name = "Free Trial Plan",
			    Description = "Acces gratuit 7 zile",
			    PlanTypeCode = "free",
			    PriceMultiplier = 0,
			    IsActive = true,
			    MaxStorageGB = 5
		    },
		    new SubscriptionPlan
		    {
			    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
			    Name = "Monthly Standard",
			    Description = "Plată lunară",
			    PlanTypeCode = "monthly",
			    PriceMultiplier = 1,
			    IsActive = true,
			    MaxStorageGB = 100
		    },
		    new SubscriptionPlan
		    {
			    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
			    Name = "Anual (Promo)",
			    Description = "Plată anuală (12 luni la preț de 10)",
			    PlanTypeCode = "annual",
			    PriceMultiplier = 10,
			    IsActive = true,
			    MaxStorageGB = 100
		    }
		);
	}

	// B. SEED ADMIN (Citit din appsettings.json)
	var adminSettings = builder.Configuration.GetSection("AdminSettings");
	string adminEmail = adminSettings["Email"] ?? "admin@adobe.com";

	if (!context.Users.Any(u => u.Email == adminEmail))
	{
		string hashedAdminPw = PasswordHasher.HashPassword(adminSettings["Password"] ?? "Admin123!");
		var adminUser = new User
		{
			Id = Guid.NewGuid(),
			Email = adminEmail,
			FullName = adminSettings["FullName"] ?? "System Admin",
			Password = hashedAdminPw,
			JoinedDate = DateTime.Now
		};
		context.Users.Add(adminUser);

		context.UserProfiles.Add(new UserProfile
		{
			Id = Guid.NewGuid(),
			UserId = adminUser.Id,
			DisplayName = "Administrator",
			Bio = "Cont de sistem principal",
			Theme = "Dark"
		});
	}

	// C. SEED APLICAȚIE DE TEST (Pentru a avea ce cumpăra în catalog)
	if (!context.CreativeApps.Any())
	{
		context.CreativeApps.Add(new CreativeApp
		{
			Id = Guid.NewGuid(),
			Name = "Photoshop",
			BasePrice = 100,
			Description = "Standardul în editare foto profesională",
			ImageUrl = "/images/apps/default-app.png"
		});
	}

	// D. SEED UTILIZATOR TEST
	if (!context.Users.Any(u => u.Email == "test@user.com"))
	{
		context.Users.Add(new User
		{
			Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
			Email = "test@user.com",
			FullName = "Test User",
			Password = PasswordHasher.HashPassword("Test"),
			JoinedDate = DateTime.Now
		});
	}

	// O SINGURĂ SALVARE LA FINAL
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
