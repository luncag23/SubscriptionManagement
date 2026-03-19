using Microsoft.AspNetCore.Mvc;
using DataContract.DTOs;
using DAL.Abstract;
using BusinessLogic.Factories;
using BusinessLogic.Facade; // <--- ADAUGĂ ACEST USING
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionManagement.Web.Controllers
{
	public class SubscriptionController : Controller
	{
		private readonly SubscriptionPurchaseFacade _purchaseFacade; // <--- 1. FOLOSIM FAȚADA
		private readonly ISubscriptionRepository _repository;

		// 2. MODIFICĂ CONSTRUCTORUL
		public SubscriptionController(SubscriptionPurchaseFacade purchaseFacade, ISubscriptionRepository repository)
		{
			_purchaseFacade = purchaseFacade;
			_repository = repository;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			// Folosim fațada pentru a lua planurile
			var plans = await _purchaseFacade.GetAvailablePlans();
			return View(plans);
		}

		[HttpPost]
		public async Task<IActionResult> Subscribe(string planType, string paymentType)
		{
			try
			{
				var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
				if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

				// 3. APELĂM FAȚADA (Mult mai simplu)
				var response = await _purchaseFacade.ExecutePurchaseFlow(planType, paymentType, Guid.Parse(userIdClaim));

				ViewBag.Message = response.Message;
				ViewBag.LicenseKey = response.LicenseKey;
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
			}

			// Re-trimitem lista prin fațadă
			var plans = await _purchaseFacade.GetAvailablePlans();
			return View("Index", plans);
		}

		// Partea de Clone rămâne la fel (folosește _repository direct pentru Prototype)
		[HttpGet]
		public async Task<IActionResult> Clone(Guid id)
		{
			var templatePlan = await _repository.GetPlanByIdAsync(id);
			if (templatePlan == null) return NotFound();
			var clonedPlan = templatePlan.Clone();
			clonedPlan.Name = "Copie - " + templatePlan.Name;
			return View("CloneEditor", clonedPlan);
		}

		[HttpPost]
		public async Task<IActionResult> ConfirmClone(Domain.Entities.SubscriptionPlan modifiedPlan)
		{
			modifiedPlan.Id = Guid.NewGuid();
			await _repository.AddSubscriptionPlanAsync(modifiedPlan);
			return RedirectToAction("Index", new { message = "Plan creat prin clonare!" });
		}
	}
}