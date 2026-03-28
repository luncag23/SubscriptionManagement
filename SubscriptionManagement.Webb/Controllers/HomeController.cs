using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.Webb.Models;
using DAL.Abstract;
using BusinessLogic.Composite;

namespace SubscriptionManagement.Webb.Controllers
{
	public class HomeController : Controller
	{
		private readonly ISubscriptionRepository _repository;
		private readonly CreativeToolAssembly _assembly;

		public HomeController(ISubscriptionRepository repository, CreativeToolAssembly assembly)
		{
			_repository = repository;
			_assembly = assembly;
		}

		public async Task<IActionResult> Index()
		{
			// Luăm doar aplicațiile individuale pentru Home
			var allApps = await _repository.GetAllAppsAsync();
			var featuredApps = allApps
				.Where(a => a.AssignmentsAsBundle == null || !a.AssignmentsAsBundle.Any())
				.Take(4)
				.ToList();

			return View(featuredApps);
		}

		// PAGINA PUBLICĂ DE PRODUSE
		public async Task<IActionResult> Products()
		{
			var allApps = (await _repository.GetAllAppsAsync()).ToList();

			// 1. FILTRARE: Aplicații individuale (Leafs)
			var individualApps = allApps
				.Where(a => a.AssignmentsAsBundle == null || !a.AssignmentsAsBundle.Any())
				.ToList();

			// 2. FILTRARE: Pachete (Composites)
			var bundleEntities = allApps
				.Where(a => a.AssignmentsAsBundle != null && a.AssignmentsAsBundle.Any())
				.ToList();

			// 3. CALCULARE PREȚURI RECURSIVE PRIN COMPOSITE
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
						Price = composite.GetPrice(),
						ImageUrl = b.ImageUrl,        // <--- ADAUGĂ ACEASTA
						Description = b.Description   // <--- ȘI ACEASTA
					});
				}
			}

			ViewBag.Bundles = calculatedBundles;

			// Trimitem lista de aplicații individuale ca Model principal al paginii
			return View(individualApps);
		}

		public IActionResult Privacy() => View();

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}