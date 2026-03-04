using DAL.Abstract;

namespace BusinessLogic.Factories
{
	//Creator principal
	public abstract class SubscriptionManager
	{
		protected readonly ISubscriptionRepository _repository;
		protected SubscriptionManager(ISubscriptionRepository repository) => _repository = repository;

		public abstract ISubscriptionActivator CreateActivator();
		public void ProcessSubscription(Guid userId, Guid planId) => CreateActivator().Activate(userId, planId);
	}

	// Creator Concret 1
	public class FreeTrialManager : SubscriptionManager
	{
		public FreeTrialManager(ISubscriptionRepository repository) : base(repository) { }
		public override ISubscriptionActivator CreateActivator() => new FreeTrialActivator(_repository);
	}

	// Creaotor Concret 2
	public class PremiumSubscriptionManager : SubscriptionManager
	{
		public PremiumSubscriptionManager(ISubscriptionRepository repository) : base(repository) { }
		public override ISubscriptionActivator CreateActivator() => new PremiumActivator(_repository);
	}
}