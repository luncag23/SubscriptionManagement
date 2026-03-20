using DAL.Abstract;

namespace BusinessLogic.Factories
{
	public abstract class SubscriptionManager
	{
		protected readonly ISubscriptionRepository _repository;
		protected SubscriptionManager(ISubscriptionRepository repository) => _repository = repository;

		public abstract ISubscriptionActivator CreateActivator();

		public void ProcessSubscription(Guid userId, Guid appId, string licenseKey)
		{
			var activator = CreateActivator();
			activator.Activate(userId, appId, licenseKey);
		}
	}

	public class FreeTrialManager : SubscriptionManager
	{
		public FreeTrialManager(ISubscriptionRepository repository) : base(repository) { }
		public override ISubscriptionActivator CreateActivator() => new FreeTrialActivator(_repository);
	}

	public class MonthlySubscriptionManager : SubscriptionManager
	{
		public MonthlySubscriptionManager(ISubscriptionRepository repository) : base(repository) { }
		public override ISubscriptionActivator CreateActivator() => new MonthlyActivator(_repository);
	}

	public class AnnualSubscriptionManager : SubscriptionManager
	{
		public AnnualSubscriptionManager(ISubscriptionRepository repository) : base(repository) { }
		public override ISubscriptionActivator CreateActivator() => new AnnualActivator(_repository);
	}
}