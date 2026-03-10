using System;
using System.Collections.Generic;
using System.Linq; // Adăugat pentru FirstOrDefault
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.AbstractFactory;
using BusinessLogic.Singleton;
using BusinessLogic.Factories; // Asigură-te că namespace-urile sunt corecte
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

			// 2. Căutăm planul ales
			var plan = allPlans.FirstOrDefault(p => p.Name.ToLower().Contains(planType.ToLower()));

			// Luăm userul de test
			var user = await _repository.GetUserByIdAsync(Guid.Parse("22222222-2222-2222-2222-222222222222"));

			if (plan == null || user == null)
			{
				throw new Exception($"Planul '{planType}' nu a fost găsit în baza de date.");
			}

			// 1. Generăm licența o SINGURĂ dată aici (Singleton)
			string finalLicense = LicenseGenerator.Instance.GenerateKey();

			// 3. FACTORY METHOD (Activarea planului)
			var manager = _provider.GetManager(planType);
			manager.ProcessSubscription(user.Id, plan.Id, finalLicense); // <--- PASĂM CHEIA AICI

			// 4. ABSTRACT FACTORY (Plată și Factură)
			var billingFactory = _billingProvider.GetFactory(paymentType);
			var processor = billingFactory.CreateProcessor();
			var invoice = billingFactory.CreateInvoice();

			string paymentResult = billingFactory.CreateProcessor().ProcessPayment(plan.MonthlyPrice);
			string invoiceResult = billingFactory.CreateInvoice().GenerateInvoice();

			// 5. SINGLETON (Generare Licență Unică)
			// Acum generăm cheia ÎNAINTE de return
			string newLicense = LicenseGenerator.Instance.GenerateKey();

			// 6. RETURN UNIC (Combinăm toate informațiile într-un singur DTO)
			return new SubscriptionResponse
			{
				PlanName = plan.Name,
				Status = "Activ",
				LicenseKey = finalLicense, // Aceeași licență salvată în DB
				Message = $"[Succes: {plan.Name}] {paymentResult} | {invoiceResult}"
			};
		}
	}
}