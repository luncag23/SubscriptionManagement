using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Composite;
using BusinessLogic.Facade;
using BusinessLogic.Factories;
using DAL.Abstract;
using DataContract.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SubscriptionManagement.Web.Controllers
{
	public class SubscriptionController : Controller
	{
		private readonly SubscriptionPurchaseFacade _purchaseFacade;
		private readonly ISubscriptionRepository _repository;
		private readonly CreativeToolAssembly _assembly;


		public SubscriptionController(
			SubscriptionPurchaseFacade purchaseFacade,
			ISubscriptionRepository repository,
			CreativeToolAssembly assembly)
		{
			_purchaseFacade = purchaseFacade;
			_repository = repository;
			_assembly = assembly;
		}

		[HttpGet]
		public async Task<IActionResult> Index(Guid? appId)
		{
			if (appId == null) return RedirectToAction("Products", "Home");

			// 1. Luăm datele produsului ales (Composite Logic)
			var creativeElement = await _assembly.LoadToolHierarchy(appId.Value);

			// 2. Luăm toate planurile de acces create în Admin (Prototype Logic)
			var allPlans = await _repository.GetAllPlansAsync();

			ViewBag.SelectedAppId = appId;
			ViewBag.SelectedProductName = creativeElement.GetName();
			ViewBag.BasePrice = creativeElement.GetPrice();

			// Trimitem lista de planuri către Model pentru a genera butoanele radio dinamic
			return View(allPlans);
		}

		[HttpPost]
		public async Task<IActionResult> Subscribe(Guid appId, Guid planId, string paymentType)
		{
			try
			{
				var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
				if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
				Guid currentUserId = Guid.Parse(userIdClaim);

				// Luăm planul din DB
				var selectedPlan = await _repository.GetPlanByIdAsync(planId);
				if (selectedPlan == null) throw new Exception("Planul nu mai există.");

				// --- CORECȚIE AICI: Adăugăm selectedPlan.Id ca al doilea argument ---
				var response = await _purchaseFacade.ExecutePurchaseFlow(
				    appId,
				    selectedPlan.Id,            // <--- ACEST ARGUMENT LIPSEA (planId)
				    selectedPlan.PlanTypeCode,
				    selectedPlan.PriceMultiplier,
				    paymentType,
				    currentUserId);

				ViewBag.Message = response.Message;
				ViewBag.LicenseKey = response.LicenseKey;
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
			}

			// Reîncărcăm datele pentru pagină (pentru a nu avea erori la re-randare)
			var creativeElement = await _assembly.LoadToolHierarchy(appId);
			var allPlans = await _repository.GetAllPlansAsync();
			ViewBag.SelectedAppId = appId;
			ViewBag.SelectedProductName = creativeElement.GetName();
			ViewBag.BasePrice = creativeElement.GetPrice();

			return View("Index", allPlans);
		}

		[HttpGet]
		public async Task<IActionResult> MyLicenses()
		{
			var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
			if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

			// Luăm toate abonamentele userului din DB
			var allSubs = await _repository.GetAllSubscriptionsAsync();
			var mySubs = allSubs.Where(s => s.UserId == Guid.Parse(userIdClaim)).ToList();
			// 2. Luăm toate planurile de acces (Prototype items) pentru a le afla numele
			var allPlans = await _repository.GetAllPlansAsync();

			var planNamesMap = allPlans.ToDictionary(p => p.Id, p => p.Name);
			ViewBag.PlanNames = planNamesMap;

			return View(mySubs);
		}

		[HttpPost]
		public async Task<IActionResult> Cancel(Guid id)
		{
			// CORECTAT: Folosim variabila care este deja injectată în constructor
			await _purchaseFacade.CancelSubscription(id);

			return RedirectToAction("MyLicenses");
		}
	}
}