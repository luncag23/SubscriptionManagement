using System.Security.Claims;
using BusinessLogic.Builder;
using DAL.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize] // Doar cei logați pot intra aici
public class ProfileController : Controller
{
	private readonly ISubscriptionRepository _repository;
	private readonly IUserProfileBuilder _builder;

	public ProfileController(ISubscriptionRepository repository, IUserProfileBuilder builder)
	{
		_repository = repository;
		_builder = builder;
	}

	[HttpGet]
	public async Task<IActionResult> Edit()
	{
		// 1. Extragem ID-ul userului logat din Claims
		var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

		// 2. Luăm profilul actual din DB (Trebuie să adaugi GetProfileByUserIdAsync în Repo)
		var profile = await _repository.GetProfileByUserIdAsync(userId);

		return View(profile);
	}

	[HttpPost]
	public async Task<IActionResult> Save(string displayName, string bio, string theme)
	{
		var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
		var oldProfile = await _repository.GetProfileByUserIdAsync(userId);

		// 1. Builder pentru profil
		_builder.Reset();
		_builder.SetUser(userId);
		_builder.SetBasicInfo(displayName, bio);
		_builder.SetPreferences(theme, true);
		var updatedProfile = _builder.GetProfile();
		updatedProfile.Id = oldProfile.Id;

		await _repository.UpdateUserProfileAsync(updatedProfile);

		// 2. ACTUALIZARE USER (Aici era eroarea)
		var user = await _repository.GetUserByIdAsync(userId);
		user.FullName = displayName;

		// SCHIMBĂ ACEASTĂ LINIE:
		await _repository.UpdateUserAsync(user); // <--- Folosim Update, NU Add

		// 3. Refresh Cookie (numele din dreapta sus)
		var claims = new List<Claim> 
		{
			new Claim(ClaimTypes.Name, displayName),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim("UserId", user.Id.ToString())
		};
		var identity = new ClaimsIdentity(claims, "CookieAuth");
		await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(identity));

		return RedirectToAction("Edit", new { message = "Profil actualizat cu succes!" });
	}
}