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

		public void Activate(Guid userId, Guid appId, string licenseKey)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				AppId = appId,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(7), // Trial 7 zile
				Status = SubscriptionStatus.Trial,
				LicenseKey = licenseKey
			};
			_repository.SaveSubscriptionAsync(sub).GetAwaiter().GetResult();
		}
	}

	// --- PRODUS 2: Abonament Lunar (30 zile) ---
	public class MonthlyActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public MonthlyActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid appId, string licenseKey)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				AppId = appId,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddMonths(1), // 1 lună
				Status = SubscriptionStatus.Active,
				LicenseKey = licenseKey
			};
			_repository.SaveSubscriptionAsync(sub).GetAwaiter().GetResult();
		}
	}

	// --- PRODUS 3: Abonament Anual (1 an) ---
	public class AnnualActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public AnnualActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid appId, string licenseKey)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				AppId = appId,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddYears(1), // 1 an
				Status = SubscriptionStatus.Active,
				LicenseKey = licenseKey
			};
			_repository.SaveSubscriptionAsync(sub).GetAwaiter().GetResult();
		}
	}
}