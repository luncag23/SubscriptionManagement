using Microsoft.AspNetCore.Mvc;
using DAL.Abstract;
using Domain.Entities;
using BusinessLogic.Builder;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using BusinessLogic.Security;

namespace SubscriptionManagement.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly ISubscriptionRepository _repository;
		private readonly IUserProfileBuilder _profileBuilder;
		private readonly ProfileDirector _director;
		private readonly IConfiguration _configuration;

		public AccountController(ISubscriptionRepository repository,
								 IUserProfileBuilder profileBuilder,
								 ProfileDirector director,
								 IConfiguration configuration)
		{
			_repository = repository;
			_profileBuilder = profileBuilder;
			_director = director;
			_configuration = configuration;
		}

		[HttpGet]
		public IActionResult Register() => View();

		[HttpPost]
		public async Task<IActionResult> Register(string email, string fullName, string password)
		{
			try
			{
				var existingUser = await _repository.GetUserByEmailAsync(email);
				if (existingUser != null)
				{
					ViewBag.Error = "Acest email este deja înregistrat!";
					return View();
				}
				// 1. Creăm entitatea de bază: User
				var newUser = new User
				{
					Id = Guid.NewGuid(),
					Email = email,
					FullName = fullName,
					Password = BusinessLogic.Security.PasswordHasher.HashPassword(password),
					JoinedDate = DateTime.Now
				};

				// 2. Salvăm User-ul (avem nevoie de ID-ul lui pentru profil)
				await _repository.AddUserAsync(newUser);

				// 3. FOLOSIM BUILDER + DIRECTOR pentru a crea Profilul
				// Directorul execută rețeta "Minimal" folosind Builder-ul
				_director.BuildMinimalProfile(newUser.Id, newUser.FullName);

				// Extragem rezultatul final din Builder
				var newUserProfile = _profileBuilder.GetProfile();

				// 4. Salvăm Profilul în baza de date
				await _repository.AddUserProfileAsync(newUserProfile);

				return RedirectToAction("Login", new { message = "Cont creat cu succes!" });
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Eroare la înregistrare: " + ex.Message;
				return View();
			}
		}

		// 1. Redenumim metoda de C#, dar păstrăm numele de acțiune "Login" pentru browser
		[HttpGet]
		[ActionName("Login")]
		public IActionResult LoginPage(string message) // Am schimbat numele în LoginPage
		{
			ViewBag.Message = message;
			return View();
		}

		// 2. Metoda de POST rămâne cu numele Login
				[HttpPost]
		public async Task<IActionResult> Login(string email, string password)
		{
			var user = await _repository.GetUserByEmailAsync(email);

			if (user != null && BusinessLogic.Security.PasswordHasher.VerifyPassword(user.Password, password))
			{
				string adminEmail = _configuration["AdminSettings:Email"];
				string role = (user.Email.ToLower() == adminEmail.ToLower()) ? "Admin" : "User";

				var claims = new List<Claim> {
					new Claim(ClaimTypes.Name, user.FullName),
					new Claim(ClaimTypes.Email, user.Email),
					new Claim("UserId", user.Id.ToString()),
					new Claim(ClaimTypes.Role, role) 
				};

				var identity = new ClaimsIdentity(claims, "CookieAuth");
				await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));

				return RedirectToAction("Index", "Home");
			}

			ViewBag.Error = "Email sau parolă incorectă.";
			return View();
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("CookieAuth");
			return RedirectToAction("Index", "Home");
		}
	}
}