using System;
using System.Collections.Generic;
using System.Linq; // Adăugat pentru FirstOrDefault
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.AbstractFactory;
using BusinessLogic.Adapters;
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

		public async Task<SubscriptionResponse> SubscribeUserAsync(string planName, string paymentType, Guid userId)
		{
			var allPlans = await _repository.GetAllPlansAsync();

			// 1. Găsim planul după numele primit din formular
			var plan = allPlans.FirstOrDefault(p => p.Name.Equals(planName, StringComparison.OrdinalIgnoreCase));
			var user = await _repository.GetUserByIdAsync(userId);

			if (plan == null || user == null) throw new Exception("Plan sau Utilizator negăsit.");

			// 2. DETERMINĂM TIPUL PENTRU FACTORY (LOGICĂ NOUĂ)
			// În loc să trimitem planName ("Free Trial Plan"), trimitem "free" sau "premium"
			string managerKey = (plan.MonthlyPrice == 0) ? "free" : "premium";

			// 3. FACTORY METHOD
			// Acum va funcționa pentru orice plan, chiar și pentru "Copie - Premium"
			var manager = _provider.GetManager(managerKey);

			// Trimitem și licența (Singleton)
			string finalLicense = LicenseGenerator.Instance.GenerateKey();
			manager.ProcessSubscription(user.Id, plan.Id, finalLicense);

			// 4. ABSTRACT FACTORY
			var billingFactory = _billingProvider.GetFactory(paymentType);
			string paymentResult = billingFactory.CreateProcessor().ProcessPayment(plan.MonthlyPrice);
			string invoiceResult = billingFactory.CreateInvoice().GenerateInvoice();

			// 3. UTILIZAREA ADAPTORULUI
			// În interiorul metodei SubscribeUserAsync
			decimal priceInEur = await _currencyAdapter.ConvertToEur(plan.MonthlyPrice);


			return new SubscriptionResponse
			{
				PlanName = plan.Name,
				Status = "Activ",
				LicenseKey = finalLicense,
				Message = $"[Succes: {plan.Name}] {paymentResult} | Total: {priceInEur:F2}€ (echivalentul a {plan.MonthlyPrice} RON) | {invoiceResult}"

				
			};
		}
	}
}