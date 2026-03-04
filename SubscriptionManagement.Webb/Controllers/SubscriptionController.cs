using Microsoft.AspNetCore.Mvc;
using DataContract.DTOs;
using DAL.Abstract;
using BusinessLogic.Factories; 

namespace SubscriptionManagement.Web.Controllers
{
	public class SubscriptionController : Controller
	{
		private readonly ISubscriptionService _subscriptionService;

		public SubscriptionController(ISubscriptionService subscriptionService)
		{
			_subscriptionService = subscriptionService;
		}

		// Afișează pagina cu formularul (lipsind în codul trimis)
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Subscribe(string planType)
		{
			try
			{
				// Controller-ul doar dă comanda și primește un DTO (DataContract)
				var response = await _subscriptionService.SubscribeUserAsync(planType);

				ViewBag.Message = response.Message;
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
			}
			return View("Index");
		}
	}
}