
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

		public void Activate(Guid userId, Guid planId)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				PlanId = planId,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(7),
				Status = SubscriptionStatus.Trial
			};
			_repository.SaveSubscriptionAsync(sub).Wait();
		}
	}

	// Concrete Product 2
	public class PremiumActivator : ISubscriptionActivator
	{
		private readonly ISubscriptionRepository _repository;
		public PremiumActivator(ISubscriptionRepository repository) => _repository = repository;

		public void Activate(Guid userId, Guid planId)
		{
			var sub = new Subscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				PlanId = planId,
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddMonths(1),
				Status = SubscriptionStatus.Active
			};
			_repository.SaveSubscriptionAsync(sub).Wait();
		}
	}
}
