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
		public async Task<SubscriptionResponse> ExecutePurchaseFlow(Guid appId, string accessType, string paymentType, Guid userId)
		{
			// Fațada doar deleagă către Service-ul care are toată logica de pattern-uri
			return await _subscriptionService.SubscribeUserAsync(appId, accessType, paymentType, userId);
		}

		// Putem pune și logica de preluare a planurilor aici pentru a curăța Controller-ul de tot
		public async Task<IEnumerable<SubscriptionPlan>> GetAvailablePlans()
		{
			return await _repository.GetAllPlansAsync();
		}
	}
}