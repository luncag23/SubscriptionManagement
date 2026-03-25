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

				// 1. Identificăm tipul de plan ales pentru a-l trimite la Factory Method
				var selectedPlan = await _repository.GetPlanByIdAsync(planId);
				string accessType = "monthly"; // default

				if (selectedPlan.MonthlyPrice == 0) accessType = "free";
				else if (selectedPlan.Name.ToLower().Contains("anual")) accessType = "annual";

				// 2. Executăm prin FAȚADĂ (care orchestrează Factory, Abstract Factory, Singleton, Adapter)
				var response = await _purchaseFacade.ExecutePurchaseFlow(appId, accessType, paymentType, currentUserId);

				ViewBag.Message = response.Message;
				ViewBag.LicenseKey = response.LicenseKey;
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
			}

			// 3. Re-încărcăm datele pentru a afișa din nou formularul (sau mesajul de succes)
			var creativeElement = await _assembly.LoadToolHierarchy(appId);
			var allPlans = await _repository.GetAllPlansAsync();

			ViewBag.SelectedAppId = appId;
			ViewBag.SelectedProductName = creativeElement.GetName();
			ViewBag.BasePrice = creativeElement.GetPrice();

			return View("Index", allPlans); // Pasăm din nou lista de planuri (Model)
		}
	}
}