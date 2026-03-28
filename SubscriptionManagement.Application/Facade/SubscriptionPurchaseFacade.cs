using BusinessLogic.Factories;
using DAL.Abstract;
using DataContract.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		// CORECTAT: Am adăugat 'Guid planId' în parametrii metodei pentru a-l putea trimite la Service
		public async Task<SubscriptionResponse> ExecutePurchaseFlow(Guid appId, Guid planId, string planTypeCode, decimal multiplier, string paymentType, Guid userId)
		{
			// Pasăm planId-ul în ordinea cerută de interfața ISubscriptionService
			return await _subscriptionService.SubscribeUserAsync(appId, planId, planTypeCode, multiplier, paymentType, userId);
		}

		public async Task<IEnumerable<SubscriptionPlan>> GetAvailablePlans()
		{
			return await _repository.GetAllPlansAsync();
		}

		public async Task CancelSubscription(Guid id)
		{
			await _subscriptionService.CancelSubscriptionAsync(id);
		}
	}
}