using Microsoft.AspNetCore.Mvc;
using DataContract.DTOs;
using DAL.Abstract;
using BusinessLogic.Factories;
using System.Linq; // Pentru FirstOrDefault
using System.Threading.Tasks;

namespace SubscriptionManagement.Web.Controllers
{
	public class SubscriptionController : Controller
	{
		private readonly ISubscriptionService _subscriptionService;
		private readonly ISubscriptionRepository _repository; // <--- 1. DECLARĂ REPOSITORY

		// 2. INJECTEAZĂ REPOSITORY ÎN CONSTRUCTOR
		public SubscriptionController(ISubscriptionService subscriptionService, ISubscriptionRepository repository)
		{
			_subscriptionService = subscriptionService;
			_repository = repository;
		}

		// 3. SCHIMBĂ ÎN ASYNC TASK
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			// Luăm toate planurile pentru a le afișa în secțiunea de Admin
			var plans = await _repository.GetAllPlansAsync();

			// Trimitem lista de planuri către View pentru a fi afișată în tabelul de clonare
			return View(plans);
		}

		[HttpGet]
		public async Task<IActionResult> Clone(Guid id)
		{
			// 1. Luăm planul-sursă din DB
			var templatePlan = await _repository.GetPlanByIdAsync(id);
			if (templatePlan == null) return NotFound();

			// 2. Aplicăm PATTERN-UL PROTOTYPE
			// Asigură-te că metoda Clone() există în clasa SubscriptionPlan din Domain
			var clonedPlan = templatePlan.Clone();

			// Îi dăm un nume sugestiv temporar
			clonedPlan.Name = "Copie - " + templatePlan.Name;

			// Trimitem CLONA către formularul de editare (View-ul CloneEditor.cshtml)
			return View("CloneEditor", clonedPlan);
		}

		[HttpPost]
		public async Task<IActionResult> ConfirmClone(Domain.Entities.SubscriptionPlan modifiedPlan)
		{
			// Ne asigurăm că are ID nou pentru a fi inserat ca rând nou
			modifiedPlan.Id = Guid.NewGuid();

			await _repository.AddSubscriptionPlanAsync(modifiedPlan);

			return RedirectToAction("Index", new { message = "Planul nou a fost creat prin clonare!" });
		}

		[HttpPost]
		public async Task<IActionResult> Subscribe(string planType, string paymentType)
		{
			try
			{
				var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
				if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

				var response = await _subscriptionService.SubscribeUserAsync(planType, paymentType, Guid.Parse(userIdClaim));

				ViewBag.Message = response.Message;
				ViewBag.LicenseKey = response.LicenseKey;
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
			}

			// IMPORTANT: Re-trimitem lista de planuri către pagină
			var plans = await _repository.GetAllPlansAsync();
			return View("Index", plans);
		}
	}
}