using BusinessLogic.Composite;
using BusinessLogic.Proxy;
using DAL.Abstract;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionManagement.Web.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly ISubscriptionRepository _repository;
		private readonly CreativeToolAssembly _assembly;
		private readonly IAdminService _adminProxy;

		public AdminController(
			ISubscriptionRepository repository,
			CreativeToolAssembly assembly,
			IAdminService adminProxy)
		{
			_repository = repository;
			_assembly = assembly;
			_adminProxy = adminProxy;
		}

		public async Task<IActionResult> Index()
		{
			var allApps = await _repository.GetAllAppsAsync();
			ViewBag.TotalApps = allApps.Count(a => a.AssignmentsAsBundle == null || !a.AssignmentsAsBundle.Any());
			ViewBag.TotalBundles = allApps.Count(a => a.AssignmentsAsBundle != null && a.AssignmentsAsBundle.Any());
			return View();
		}

		// --- CATALOG ---
		[HttpGet]
		public async Task<IActionResult> Catalog()
		{
			var allApps = await _repository.GetAllAppsAsync();
			var displayBundles = new List<dynamic>();

			var bundleEntities = allApps.Where(a => a.AssignmentsAsBundle.Any()).ToList();
			foreach (var b in bundleEntities)
			{
				var composite = await _assembly.LoadToolHierarchy(b.Id);
				displayBundles.Add(new
				{
					Id = b.Id,
					Name = composite.GetName(),
					Price = composite.GetPrice(),
					ImageUrl = b.ImageUrl // <--- ACEASTA TREBUIE SĂ EXISTE AICI
				});
			}

			ViewBag.CalculatedBundles = displayBundles;
			return View(allApps.Where(a => !a.AssignmentsAsBundle.Any()).ToList());
		}

		// --- GESTIUNE APLICAȚII (LEAFS) ---

		[HttpGet]
		public IActionResult CreateApp() => View();

		[HttpPost]
		public async Task<IActionResult> CreateApp(string name, decimal price, string description, IFormFile imageFile)
		{
			try
			{
				// 1. Procesăm imaginea
				string imageUrl = await UploadImage(imageFile) ?? "/images/apps/default-app.png";

				// 2. Apelăm DOAR Proxy-ul (el va apela repository-ul în spate dacă ești Admin)
				// Am adăugat description și imageUrl în apel
				await _adminProxy.CreateApp(name, price, description, imageUrl);

				return RedirectToAction("Catalog");
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				return View();
			}
		}

		[HttpGet]
		public async Task<IActionResult> EditApp(Guid id)
		{
			var app = await _repository.GetAppByIdAsync(id);
			if (app == null) return NotFound();
			return View(app);
		}

		[HttpPost]
		public async Task<IActionResult> EditApp(CreativeApp model, IFormFile imageFile)
		{
			var app = await _repository.GetAppByIdAsync(model.Id);
			if (app == null) return NotFound();

			app.Name = model.Name;
			app.BasePrice = model.BasePrice;
			app.Description = model.Description;

			// Dacă s-a încărcat o imagine nouă, o înlocuim
			var newImageUrl = await UploadImage(imageFile);
			if (newImageUrl != null) app.ImageUrl = newImageUrl;

			await _repository.UpdateAppAsync(app);
			return RedirectToAction("Catalog");
		}

		// --- GESTIUNE PACHETE (COMPOSITES) ---

		[HttpGet]
		public IActionResult CreateBundle() => View();

		// --- GESTIUNE PACHETE (POST) ---

		[HttpPost]
		public async Task<IActionResult> CreateBundle(string name, string description, IFormFile imageFile)
		{
			try
			{
				// 1. Upload imagine (folosind metoda helper de data trecută)
				string imageUrl = await UploadImage(imageFile) ?? "/images/apps/default-bundle.png";

				// 2. Creăm pachetul via Proxy
				var newId = await _adminProxy.CreateBundle(name, description);

				// 3. Actualizăm imaginea (deoarece CreateBundle din Proxy pune default)
				var bundle = await _repository.GetAppByIdAsync(newId);
				bundle.ImageUrl = imageUrl;
				await _repository.UpdateAppAsync(bundle);

				return RedirectToAction("EditBundle", new { id = newId });
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				return View();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditBundle(CreativeApp model, IFormFile imageFile, List<Guid> selectedAppIds)
		{
			var bundle = await _repository.GetAppByIdAsync(model.Id);
			if (bundle == null) return NotFound();

			bundle.Name = model.Name;
			bundle.Description = model.Description;

			// Upload imagine nouă dacă a fost selectată
			var newImagePath = await UploadImage(imageFile);
			if (newImagePath != null) bundle.ImageUrl = newImagePath;

			await _adminProxy.UpdateApp(bundle, selectedAppIds);
			return RedirectToAction("Catalog");
		}

		[HttpGet]
		public async Task<IActionResult> EditBundle(Guid id)
		{
			var bundle = await _repository.GetAppByIdAsync(id);
			if (bundle == null) return NotFound();

			// 1. Luăm TOATE aplicațiile din baza de date
			var allApps = await _repository.GetAllAppsAsync();

			// 2. FILTRARE: Arătăm tot în afară de pachetul în care ne aflăm (să nu se conțină pe sine)
			// De asemenea, ne asigurăm că 'allApps' nu este null
			ViewBag.AvailableApps = allApps.Where(a => a.Id != id).ToList();

			// 3. Luăm copiii actuali pentru a bifa checkbox-urile
			var currentChildren = await _repository.GetChildrenForBundleAsync(id);
			ViewBag.CurrentChildrenIds = currentChildren.Select(c => c.Id).ToList();

			// 4. TRIMITEM modelul 'bundle' către pagină pentru datele de bază (nume/descriere)
			return View(bundle);
		}

		

		// --- ALTELE ---

		[HttpGet]
		public async Task<IActionResult> SubscriptionPlans()
		{
			return View(await _repository.GetAllPlansAsync());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteElement(Guid id)
		{
			await _repository.DeleteAppAsync(id);
			return RedirectToAction("Catalog");
		}

		// METODĂ PRIVATĂ PENTRU UPLOAD (Helper)
		private async Task<string> UploadImage(IFormFile imageFile)
		{
			if (imageFile == null || imageFile.Length == 0) return null;

			string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/apps");
			if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

			string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
			string filePath = Path.Combine(folder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await imageFile.CopyToAsync(stream);
			}
			return "/images/apps/" + fileName;
		}

		[HttpGet]
		public IActionResult Settings() => View();
	}
}