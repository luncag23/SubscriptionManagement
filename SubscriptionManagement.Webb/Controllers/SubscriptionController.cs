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
		private readonly CreativeToolAssembly _assembly; // <--- 1. ADAUGĂ CÂMPUL PENTRU ASSEMBLY

		// 2. INJECTEAZĂ ȘI ASSEMBLY ÎN CONSTRUCTOR
		public SubscriptionController(
			SubscriptionPurchaseFacade purchaseFacade,
			ISubscriptionRepository repository,
			CreativeToolAssembly assembly) // <--- Injectăm aici
		{
			_purchaseFacade = purchaseFacade;
			_repository = repository;
			_assembly = assembly; // <--- Atribuim aici
		}

		[HttpGet]
		public async Task<IActionResult> Index(Guid? appId)
		{
			if (appId == null) return RedirectToAction("Products", "Home");

			// 3. FOLOSEȘTE _assembly (deja injectat), nu mai face 'new'
			var product = await _assembly.LoadToolHierarchy(appId.Value);

			ViewBag.SelectedProductName = product.GetName();
			ViewBag.SelectedAppId = appId;
			ViewBag.BasePrice = product.GetPrice();

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Subscribe(Guid appId, string accessType, string paymentType)
		{
			try
			{
				var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
				if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");
				Guid currentUserId = Guid.Parse(userIdClaim);

				var response = await _purchaseFacade.ExecutePurchaseFlow(appId, accessType, paymentType, currentUserId);

				ViewBag.Message = response.Message;
				ViewBag.LicenseKey = response.LicenseKey;
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
			}

			// 4. ACUM _assembly ESTE DISPONIBILĂ ȘI AICI FĂRĂ EROARE
			var creativeElement = await _assembly.LoadToolHierarchy(appId);
			ViewBag.SelectedProductName = creativeElement.GetName();
			ViewBag.BasePrice = creativeElement.GetPrice();
			ViewBag.SelectedAppId = appId;

			return View("Index");
		}

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