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
	[Authorize]
	public class AdminController : Controller
	{
		private readonly ISubscriptionRepository _repository;
		private readonly CreativeToolAssembly _assembly;

		public AdminController(ISubscriptionRepository repository, CreativeToolAssembly assembly)
		{
			_repository = repository;
			_assembly = assembly;
		}

		// --- CATALOG (PAGINA PRINCIPALĂ ADMIN) ---
		[HttpGet]
		public async Task<IActionResult> Catalog()
		{
			var allApps = await _repository.GetAllAppsAsync();

			// 1. FILTRARE STÂNGA: Doar aplicațiile individuale (Leafs)
			// Un element este aplicație dacă NU conține alte elemente (AssignmentsAsBundle e gol)
			var individualApps = allApps
				.Where(a => a.AssignmentsAsBundle == null || !a.AssignmentsAsBundle.Any())
				.ToList();

			// 2. FILTRARE DREAPTA: Doar pachetele (Composites)
			// Un element este pachet dacă conține cel puțin un alt element
			var bundleEntities = allApps
				.Where(a => a.AssignmentsAsBundle != null && a.AssignmentsAsBundle.Any())
				.ToList();

			var calculatedBundles = new List<dynamic>();
			foreach (var b in bundleEntities)
			{
				// Folosim motorul de asamblare Composite pentru a calcula prețul recursiv
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

		// --- GESTIUNE APLICAȚII (LEAFS) ---

		[HttpGet]
		public IActionResult CreateApp() => View();

		[HttpPost]
		public async Task<IActionResult> CreateApp(string name, decimal price)
		{
			var newApp = new CreativeApp
			{
				Id = Guid.NewGuid(),
				Name = name,
				BasePrice = price
			};
			await _repository.AddAppAsync(newApp);
			return RedirectToAction("Catalog", new { message = "Aplicație creată!" });
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

		// --- GESTIUNE PACHETE (COMPOSITES) ---

		[HttpGet]
		public IActionResult CreateBundle() => View();

		[HttpPost]
		public async Task<IActionResult> CreateBundle(string name)
		{
			// Un pachet este o CreativeApp cu preț de bază 0 (prețul se va calcula din copii)
			var newBundle = new CreativeApp
			{
				Id = Guid.NewGuid(),
				Name = name,
				BasePrice = 0
			};
			await _repository.AddAppAsync(newBundle);

			// Redirecționăm la EditBundle pentru a alege ce conține
			return RedirectToAction("EditBundle", new { id = newBundle.Id });
		}

		[HttpGet]
		public async Task<IActionResult> EditBundle(Guid id)
		{
			var bundle = await _repository.GetAppByIdAsync(id);
			if (bundle == null) return NotFound();

			var allApps = await _repository.GetAllAppsAsync();

			// Pentru a evita buclele infinite, un pachet poate conține doar 
			// aplicații care NU sunt ele însele pachete în acest moment
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
			try
			{
				var bundle = await _repository.GetAppByIdAsync(id);
				if (bundle == null) return NotFound();

				bundle.Name = name;
				await _repository.UpdateAppAsync(bundle);

				// Actualizăm legăturile Many-to-Many
				await _repository.UpdateBundleAssignmentsAsync(id, selectedAppIds);

				return RedirectToAction("Catalog");
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Eroare la salvarea pachetului: " + ex.Message;
				return View();
			}
		}

		[HttpPost]
		public async Task<IActionResult> DeleteApp(Guid id)
		{
			// Logica de ștergere (opțional de implementat în Repo)
			return RedirectToAction("Catalog");
		}

		[HttpPost]
		[ValidateAntiForgeryToken] // Recomandat pentru securitate la ștergere
		public async Task<IActionResult> DeleteElement(Guid id)
		{
			try
			{
				await _repository.DeleteAppAsync(id);
				return RedirectToAction("Catalog", new { message = "Element șters cu succes!" });
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Nu s-a putut șterge: " + ex.Message;
				return RedirectToAction("Catalog");
			}
		}
	}
}