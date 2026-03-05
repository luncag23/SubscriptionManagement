using System;
using System.Collections.Generic;
using System.Text;
using BusinessLogic.AbstractFactory;
using DAL.Abstract;
using DataContract.DTOs;

namespace BusinessLogic.Factories
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _repository;
		private readonly SubscriptionProvider _provider;
		private readonly BillingProvider _billingProvider;

		public SubscriptionService(ISubscriptionRepository repository, SubscriptionProvider provider, BillingProvider billingProvider)
		{
			_repository = repository;
			_provider = provider;
			_billingProvider = billingProvider;
		}

		public async Task<SubscriptionResponse> SubscribeUserAsync(string planType, string paymentType)
		{
			// 1. Luăm toate planurile din DB
			var allPlans = await _repository.GetAllPlansAsync();

			// 2. FILTRARE: Căutăm planul care conține în nume textul selectat (ex: "free" sau "premium")
			// Folosim ToLower() pentru a fi siguri că găsește indiferent de litere mari/mici
			var plan = allPlans.FirstOrDefault(p => p.Name.ToLower().Contains(planType.ToLower()));

			// Luăm userul de test
			var user = await _repository.GetUserByIdAsync(Guid.Parse("22222222-2222-2222-2222-222222222222"));

			if (plan == null || user == null)
			{
				throw new Exception($"Planul '{planType}' nu a fost găsit în baza de date.");
			}

			// 3. FACTORY METHOD (Activarea planului corect)
			var manager = _provider.GetManager(planType);
			manager.ProcessSubscription(user.Id, plan.Id);

			// 4. ABSTRACT FACTORY (Plată și Factură)
			var billingFactory = _billingProvider.GetFactory(paymentType);
			var processor = billingFactory.CreateProcessor();
			var invoice = billingFactory.CreateInvoice();

			// Acum plan.MonthlyPrice va fi 0 pentru Free și 50 pentru Premium!
			string paymentResult = processor.ProcessPayment(plan.MonthlyPrice);
			string invoiceResult = invoice.GenerateInvoice();

			return new SubscriptionResponse
			{
				Message = $"[Succes: {plan.Name}] {paymentResult} | {invoiceResult}"
			};
		}
	}
}
