
using DAL.Abstract;
using DataContract.Enums;
using Domain;
using Domain.Entities;

namespace BusinessLogic.Factories
{

	// Concrete Product 1
	public class FreeTrialActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public FreeTrialActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid planId, string licenseKey)
		{
			// Generăm cheia folosind Singleton-ul creat anterior
			string generatedKey = BusinessLogic.Singleton.LicenseGenerator.Instance.GenerateKey();

			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				PlanId = planId,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddMonths(1),
				Status = SubscriptionStatus.Active,
				LicenseKey = licenseKey // <--- ACUM SE SALVEAZĂ ÎN DB!
			};

			_repository.SaveSubscriptionAsync(sub).Wait();
		}
	}

	// Concrete Product 2
	public class PremiumActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public PremiumActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid planId, string licenseKey)
		{
			// ASIGURĂ-TE CĂ GENEREZI LICENȚA ȘI AICI!
			string key = BusinessLogic.Singleton.LicenseGenerator.Instance.GenerateKey();

			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				PlanId = planId, // Folosește planId-ul primit ca argument!
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddMonths(1),
				Status = SubscriptionStatus.Active,
				LicenseKey = licenseKey // <--- Folosește cheia primită de la Service!
			};
			_repository.SaveSubscriptionAsync(sub).GetAwaiter().GetResult();
		}
	}
}
