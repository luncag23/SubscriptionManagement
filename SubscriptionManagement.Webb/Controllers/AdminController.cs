using BusinessLogic.Composite;
using DAL.Abstract;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionManagement.Web.Controllers
{
	[Authorize] // Momentan accesibil pentru orice utilizator logat (ulterior restricționat prin Proxy/Role)
	public class AdminController : Controller
	{
		private readonly ISubscriptionRepository _repository;
		private readonly CreativeToolAssembly _assembly;

		public AdminController(ISubscriptionRepository repository, CreativeToolAssembly assembly)
		{
			_repository = repository;
			_assembly = assembly;
		}

		// --- DASHBOARD ---
		public async Task<IActionResult> Index()
		{
			var allApps = await _repository.GetAllAppsAsync();
			ViewBag.TotalApps = allApps.Count(a => a.AssignmentsAsBundle == null || !a.AssignmentsAsBundle.Any());
			ViewBag.TotalBundles = allApps.Count(a => a.AssignmentsAsBundle != null && a.AssignmentsAsBundle.Any());

			return View();
		}

		[HttpGet]
		public IActionResult Settings()
		{
			return View();
		}

		// --- GESTIUNE PLANURI DE ACCES (PROTOTYPE PATTERN) ---

		[HttpGet]
		public async Task<IActionResult> SubscriptionPlans()
		{
			var plans = await _repository.GetAllPlansAsync();

			// Debug rapid: Pune un breakpoint aici să vezi dacă 'plans' are elemente
			return View(plans);
		}

		[HttpGet]
		public async Task<IActionResult> ClonePlan(Guid id)
		{
			var originalPlan = await _repository.GetPlanByIdAsync(id);
			if (originalPlan == null) return NotFound();

			// EXECUTĂM PROTOTYPE PATTERN:
			// Obiectul original se clonează singur în memorie
			var clonedPlan = originalPlan.Clone();

			// Sugerăm un nume nou pentru a diferenția clona
			clonedPlan.Name = "Copie - " + originalPlan.Name;

			// Trimitem clona către View pentru a fi editată înainte de salvarea finală
			return View(clonedPlan);
		}

		[HttpPost]
		public async Task<IActionResult> SaveClonedPlan(SubscriptionPlan model)
		{
			try
			{
				// Resetăm ID-ul pentru a fi siguri că EF îl tratează ca o entitate nouă
				model.Id = Guid.NewGuid();

				await _repository.AddSubscriptionPlanAsync(model);
				return RedirectToAction("SubscriptionPlans", new { message = "Plan creat prin clonare!" });
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Eroare la salvarea clonei: " + ex.Message;
				return View("ClonePlan", model);
			}
		}

		// --- CATALOG PRODUSE (COMPOSITE PATTERN) ---

		[HttpGet]
		public async Task<IActionResult> Catalog()
		{
			var allApps = await _repository.GetAllAppsAsync();

			// Filtrare Stânga: Aplicații (Frunze)
			var individualApps = allApps
				.Where(a => a.AssignmentsAsBundle == null || !a.AssignmentsAsBundle.Any())
				.ToList();

			// Filtrare Dreapta: Pachete (Compozite)
			var bundleEntities = allApps
				.Where(a => a.AssignmentsAsBundle != null && a.AssignmentsAsBundle.Any())
				.ToList();

			var calculatedBundles = new List<dynamic>();
			foreach (var b in bundleEntities)
			{
				var composite = await _assembly.LoadToolHierarchy(b.Id);
				if (composite != null)
				{
					calculatedBundles.Add(new
					{
						Id = b.Id,
						Name = composite.GetName(),
						Price = composite.GetPrice()
					});
				}
			}

			ViewBag.CalculatedBundles = calculatedBundles;
			return View(individualApps);
		}

		// --- CRUD APLICAȚII ---

		[HttpGet]
		public IActionResult CreateApp() => View();

		[HttpPost]
		public async Task<IActionResult> CreateApp(string name, decimal price)
		{
			var newApp = new CreativeApp { Id = Guid.NewGuid(), Name = name, BasePrice = price };
			await _repository.AddAppAsync(newApp);
			return RedirectToAction("Catalog");
		}

		[HttpGet]
		public async Task<IActionResult> EditApp(Guid id)
		{
			var app = await _repository.GetAppByIdAsync(id);
			if (app == null) return NotFound();
			return View(app);
		}

		[HttpPost]
		public async Task<IActionResult> EditApp(CreativeApp model)
		{
			var app = await _repository.GetAppByIdAsync(model.Id);
			if (app == null) return NotFound();

			app.Name = model.Name;
			app.BasePrice = model.BasePrice;

			await _repository.UpdateAppAsync(app);
			return RedirectToAction("Catalog");
		}

		// --- CRUD PACHETE ---

		[HttpGet]
		public IActionResult CreateBundle() => View();

		[HttpPost]
		public async Task<IActionResult> CreateBundle(string name)
		{
			var newBundle = new CreativeApp { Id = Guid.NewGuid(), Name = name, BasePrice = 0 };
			await _repository.AddAppAsync(newBundle);
			return RedirectToAction("EditBundle", new { id = newBundle.Id });
		}

		[HttpGet]
		public async Task<IActionResult> EditBundle(Guid id)
		{
			var bundle = await _repository.GetAppByIdAsync(id);
			if (bundle == null) return NotFound();

			var allApps = await _repository.GetAllAppsAsync();
			ViewBag.AvailableApps = allApps
				.Where(a => a.Id != id && (a.AssignmentsAsBundle == null || !a.AssignmentsAsBundle.Any()))
				.ToList();

			var currentChildren = await _repository.GetChildrenForBundleAsync(id);
			ViewBag.CurrentChildrenIds = currentChildren.Select(c => c.Id).ToList();

			return View(bundle);
		}

		[HttpPost]
		public async Task<IActionResult> EditBundle(Guid id, string name, List<Guid> selectedAppIds)
		{
			var bundle = await _repository.GetAppByIdAsync(id);
			bundle.Name = name;
			await _repository.UpdateAppAsync(bundle);
			await _repository.UpdateBundleAssignmentsAsync(id, selectedAppIds);

			return RedirectToAction("Catalog");
		}

		// --- ȘTERGERE ---

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteElement(Guid id)
		{
			try
			{
				await _repository.DeleteAppAsync(id);
				return RedirectToAction("Catalog", new { message = "Șters cu succes!" });
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Eroare la ștergere: " + ex.Message;
				return RedirectToAction("Catalog");
			}
		}
	}
}