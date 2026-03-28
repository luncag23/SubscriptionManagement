using DAL.Abstract;
using DataContract.Enums;
using Domain.Entities;
using System;

namespace BusinessLogic.Factories
{
	// --- PRODUS 1: Free Trial (7 zile) ---
	public class FreeTrialActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public FreeTrialActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid appId, Guid planId, string licenseKey, DateTime startDate)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				AppId = appId,
				PlanId = planId, // <--- ACEASTA ESTE LINIA CARE ÎȚI LIPSEȘTE!
				LicenseKey = licenseKey,
				StartDate = startDate,
				EndDate = startDate.AddDays(7), // Începe de la data primită
				Status = SubscriptionStatus.Trial,
				
			};
			_repository.SaveSubscriptionAsync(sub).GetAwaiter().GetResult();
		}
	}

	// --- PRODUS 2: Abonament Lunar (30 zile) ---
	public class MonthlyActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public MonthlyActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid appId, Guid planId, string licenseKey, DateTime startDate)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				AppId = appId,
				PlanId = planId, // <--- ACEASTA ESTE LINIA CARE ÎȚI LIPSEȘTE!
				LicenseKey = licenseKey,
				StartDate = startDate,
				EndDate = startDate.AddMonths(1),
				Status = SubscriptionStatus.Active
			};

			_repository.SaveSubscriptionAsync(sub).GetAwaiter().GetResult();
		}
	}

	// --- PRODUS 3: Abonament Anual (1 an) ---
	public class AnnualActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public AnnualActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid appId, Guid planId, string licenseKey, DateTime startDate)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				AppId = appId,
				PlanId = planId, // <--- ACEASTA ESTE LINIA CARE ÎȚI LIPSEȘTE!
				LicenseKey = licenseKey,
				StartDate = startDate,
				EndDate = startDate.AddYears(1),
				Status = SubscriptionStatus.Active,
				
			};
			_repository.SaveSubscriptionAsync(sub).GetAwaiter().GetResult();
		}
	}
}