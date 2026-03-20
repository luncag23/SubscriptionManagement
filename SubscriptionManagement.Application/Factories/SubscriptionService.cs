using System;
using System.Collections.Generic;
using System.Linq; // Adăugat pentru FirstOrDefault
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.AbstractFactory;
using BusinessLogic.Adapters;
using BusinessLogic.Composite;
using BusinessLogic.Factories; // Asigură-te că namespace-urile sunt corecte
using BusinessLogic.Singleton;
using DAL.Abstract;
using DataContract.DTOs;
using Domain.Entities;

namespace BusinessLogic.Factories
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _repository;
		private readonly SubscriptionProvider _provider;
		private readonly BillingProvider _billingProvider;
		private readonly ICurrencyConverter _currencyAdapter;

		public SubscriptionService(ISubscriptionRepository repository, SubscriptionProvider provider, BillingProvider billingProvider, ICurrencyConverter currencyAdapter)
		{
			_repository = repository;
			_provider = provider;
			_billingProvider = billingProvider;
			_currencyAdapter = currencyAdapter;
		}

		public async Task<SubscriptionResponse> SubscribeUserAsync(Guid appId, string accessType, string paymentType, Guid userId)
		{
			// 1. FOLOSIM COMPOSITE: Calculăm prețul real al produsului ales (App sau Bundle)
			var assembly = new CreativeToolAssembly(_repository);
			var creativeElement = await assembly.LoadToolHierarchy(appId);

			if (creativeElement == null) throw new Exception("Produsul nu există!");

			decimal basePrice = creativeElement.GetPrice();

			// 2. LOGICA DE FACTORY: Calculăm prețul final în funcție de tipul de acces
			decimal finalPrice = accessType switch
			{
				"free" => 0,
				"monthly" => basePrice,
				"annual" => basePrice * 10, // Ofertă: 12 luni la preț de 10
				_ => basePrice
			};

			// 3. ACTIVARE PRIN FACTORY METHOD
			var manager = _provider.GetManager(accessType);
			string licenseKey = LicenseGenerator.Instance.GenerateKey();
			manager.ProcessSubscription(userId, appId, licenseKey);

			// 4. PLATA PRIN ABSTRACT FACTORY (Folosim prețul calculat dinamic!)
			var billingFactory = _billingProvider.GetFactory(paymentType);
			var paymentMsg = billingFactory.CreateProcessor().ProcessPayment(finalPrice);

			// 5. CONVERSIE PRIN ADAPTER (Pentru raportare în EUR)
			decimal priceInEur = await _currencyAdapter.ConvertToEur(finalPrice);

			return new SubscriptionResponse
			{
				PlanName = $"{creativeElement.GetName()} ({accessType})",
				LicenseKey = licenseKey,
				Message = $"Succes! {paymentMsg} | Total: {priceInEur:F2}€ ({finalPrice} RON)"
			};
		}
	}
}