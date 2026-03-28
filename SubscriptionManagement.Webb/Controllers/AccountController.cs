using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BusinessLogic.Builder;
using BusinessLogic.Strategies;
using DAL.Abstract;
using Domain.Entities;
using BusinessLogic.Security;

namespace SubscriptionManagement.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly ISubscriptionRepository _repository;
		private readonly IUserProfileBuilder _profileBuilder;
		private readonly ProfileDirector _director;
		private readonly IConfiguration _configuration;
		private readonly AuthProvider _authProvider;

		public AccountController(ISubscriptionRepository repository,
								 IUserProfileBuilder profileBuilder,
								 ProfileDirector director,
								 IConfiguration configuration,
								 AuthProvider authProvider)
		{
			_repository = repository;
			_profileBuilder = profileBuilder;
			_director = director;
			_configuration = configuration;
			_authProvider = authProvider;
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

				var newUser = new User
				{
					Id = Guid.NewGuid(),
					Email = email,
					FullName = fullName,
					Password = PasswordHasher.HashPassword(password),
					JoinedDate = DateTime.Now
				};

				await _repository.AddUserAsync(newUser);
				_director.BuildMinimalProfile(newUser.Id, newUser.FullName);
				var newUserProfile = _profileBuilder.GetProfile();
				await _repository.AddUserProfileAsync(newUserProfile);

				return RedirectToAction("Login", new { message = "Cont creat cu succes!" });
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Eroare la înregistrare: " + ex.Message;
				return View();
			}
		}

		// --- ACEASTA ESTE METODA CARE LIPSEA (Rezultă în 405 la accesare URL) ---
		[HttpGet]
		public IActionResult Login(string message)
		{
			ViewBag.Message = message;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string? email, string? password, string provider = "local")
		{
			try
			{
				// Protecție: dacă e login local și email e null
				if (provider == "local" && string.IsNullOrEmpty(email))
				{
					ViewBag.Error = "Te rugăm să introduci email-ul.";
					return View();
				}

				// 1. Alegem strategia (Strategy Pattern)
				var strategy = _authProvider.GetStrategy(provider);

				// 2. Executăm autentificarea (pentru google/fb email-ul este simulat în form)
				var result = await strategy.AuthenticateAsync(email ?? "social-auth", password ?? "");

				if (result.Success)
				{
					var user = await _repository.GetUserByEmailAsync(result.Email);

					Guid userId = user?.Id ?? Guid.NewGuid();
					string fullName = user?.FullName ?? result.FullName;

					string adminEmail = _configuration["AdminSettings:Email"];
					string role = (result.Email.ToLower() == adminEmail.ToLower()) ? "Admin" : "User";

					var claims = new List<Claim> {
						new Claim(ClaimTypes.Name, fullName),
						new Claim(ClaimTypes.Email, result.Email),
						new Claim("UserId", userId.ToString()),
						new Claim(ClaimTypes.Role, role)
					};

					var identity = new ClaimsIdentity(claims, "CookieAuth");
					await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));

					return RedirectToAction("Index", "Home");
				}

				ViewBag.Error = result.ErrorMessage ?? "Email sau parolă incorectă.";
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Eroare de sistem: " + ex.Message;
			}

			return View();
		}

		// În AccountController.cs

		// 1. Trimite cererea către Google
		[HttpGet]
		public IActionResult ExternalLogin(string provider)
		{
			var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, provider); // "provider" va fi "Google"
		}

		// 2. Primește răspunsul de la Google
		public async Task<IActionResult> ExternalLoginCallback()
		{
			// Luăm datele trimise de Google
			var result = await HttpContext.AuthenticateAsync("CookieAuth");
			if (!result.Succeeded) return RedirectToAction("Login");

			var email = result.Principal.FindFirstValue(ClaimTypes.Email);
			var name = result.Principal.FindFirstValue(ClaimTypes.Name);

			// VERIFICĂM / SALVĂM ÎN BAZA DE DATE
			var user = await _repository.GetUserByEmailAsync(email);

			if (user == null)
			{
				// UTILIZATOR NOU -> Folosim BUILDER PATTERN (ca la Register)
				user = new User
				{
					Id = Guid.NewGuid(),
					Email = email,
					FullName = name,
					Password = "EXTERNAL_AUTH_" + Guid.NewGuid().ToString(), // Parolă dummy securizată
					JoinedDate = DateTime.Now
				};
				await _repository.AddUserAsync(user);

				// Creăm profilul via Builder & Director
				_director.BuildMinimalProfile(user.Id, user.FullName);
				var profile = _profileBuilder.GetProfile();
				await _repository.AddUserProfileAsync(profile);
			}

			// Actualizăm Claim-urile pentru a include UserID-ul nostru intern și Rolul
			var adminEmail = _configuration["AdminSettings:Email"];
			var claims = new List<Claim> {
		new Claim(ClaimTypes.Name, user.FullName),
		new Claim(ClaimTypes.Email, user.Email),
		new Claim("UserId", user.Id.ToString()),
		new Claim(ClaimTypes.Role, (user.Email.ToLower() == adminEmail.ToLower()) ? "Admin" : "User")
	};

			var identity = new ClaimsIdentity(claims, "CookieAuth");
			await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));

			return RedirectToAction("Index", "Home");
		}

		[HttpPost] // Logout trebuie să fie POST pentru securitate
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("CookieAuth");
			return RedirectToAction("Index", "Home");
		}
	}
}