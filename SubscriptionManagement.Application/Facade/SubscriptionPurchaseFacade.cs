using BusinessLogic.Factories;
using DAL.Abstract;
using DataContract.DTOs;
using Domain.Entities;

namespace BusinessLogic.Facade
{
	public class SubscriptionPurchaseFacade
	{
		private readonly ISubscriptionService _subscriptionService;
		private readonly ISubscriptionRepository _repository;

		public SubscriptionPurchaseFacade(ISubscriptionService subscriptionService, ISubscriptionRepository repository)
		{
			_subscriptionService = subscriptionService;
			_repository = repository;
		}

		// Aceasta este metoda simplificată ("Fațada")
		public async Task<SubscriptionResponse> ExecutePurchaseFlow(string planType, string paymentType, Guid userId)
		{
			// Aici ascundem complexitatea apelului către service
			return await _subscriptionService.SubscribeUserAsync(planType, paymentType, userId);
		}

		// Putem pune și logica de preluare a planurilor aici pentru a curăța Controller-ul de tot
		public async Task<IEnumerable<SubscriptionPlan>> GetAvailablePlans()
		{
			return await _repository.GetAllPlansAsync();
		}
	}
}