using BusinessLogic.AbstractFactory;
using BusinessLogic.Adapters;
using BusinessLogic.Composite;
using BusinessLogic.Singleton;
using DAL.Abstract;
using DataContract.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Factories
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _repository;
		private readonly SubscriptionProvider _provider;
		private readonly BillingProvider _billingProvider;
		private readonly ICurrencyConverter _currencyAdapter;
		private readonly CreativeToolAssembly _assembly;

		public SubscriptionService(ISubscriptionRepository repository,
						  SubscriptionProvider provider,
						  BillingProvider billingProvider,
						  ICurrencyConverter currencyAdapter,
						  CreativeToolAssembly assembly)
		{
			_repository = repository;
			_provider = provider;
			_billingProvider = billingProvider;
			_currencyAdapter = currencyAdapter;
			_assembly = assembly;
		}

		// CORECTAT: Am adăugat 'Guid planId' în parametrii metodei
		public async Task<SubscriptionResponse> SubscribeUserAsync(Guid appId, Guid planId, string planTypeCode, decimal multiplier, string paymentType, Guid userId)
		{
			var user = await _repository.GetUserByIdAsync(userId);
			var creativeElement = await _assembly.LoadToolHierarchy(appId);

			if (user == null || creativeElement == null) throw new Exception("Utilizator sau Produs negăsit.");

			// 1. LOGICĂ DE PRELUNGIRE (STACKING)
			var allSubs = await _repository.GetAllSubscriptionsAsync();
			var latestActiveSub = allSubs
			    .Where(s => s.UserId == userId && s.AppId == appId && s.EndDate > DateTime.Now)
			    .OrderByDescending(s => s.EndDate)
			    .FirstOrDefault();

			DateTime finalStartDate = latestActiveSub != null ? latestActiveSub.EndDate : DateTime.Now;

			// 2. CALCUL PREȚ DINAMIC
			decimal basePrice = creativeElement.GetPrice();
			decimal finalPrice = basePrice * multiplier;

			// 3. GENERARE LICENȚĂ (Singleton)
			string finalLicense = LicenseGenerator.Instance.GenerateKey();

			// 4. ACTIVARE PRIN FACTORY METHOD
			// Acum 'planId' este recunoscut pentru că l-am adăugat în semnătura de sus
			var manager = _provider.GetManager(planTypeCode);
			manager.ProcessSubscription(userId, appId, planId, finalLicense, finalStartDate);

			// 5. PLATA ȘI CONVERSIE
			var billingFactory = _billingProvider.GetFactory(paymentType);
			string paymentResult = billingFactory.CreateProcessor().ProcessPayment(finalPrice);
			decimal priceInEur = await _currencyAdapter.ConvertToEur(finalPrice);

			return new SubscriptionResponse
			{
				PlanName = $"{creativeElement.GetName()} ({planTypeCode})",
				LicenseKey = finalLicense,
				Message = $"Succes! {paymentResult} | Total: {priceInEur:F2}€ ({finalPrice:F2} RON)"
			};
		}

		public async Task CancelSubscriptionAsync(Guid subscriptionId)
		{
			var sub = await _repository.GetSubscriptionByIdAsync(subscriptionId);
			if (sub != null)
			{
				sub.Status = DataContract.Enums.SubscriptionStatus.Canceled;
				await _repository.UpdateSubscriptionAsync(sub);
			}
		}
	}
}