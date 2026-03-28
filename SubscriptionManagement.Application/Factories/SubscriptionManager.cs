using DAL.Abstract;

namespace BusinessLogic.Factories
{
	public abstract class SubscriptionManager
	{
		protected readonly ISubscriptionRepository _repository;
		protected SubscriptionManager(ISubscriptionRepository repository) => _repository = repository;

		public abstract ISubscriptionActivator CreateActivator();

		public void ProcessSubscription(Guid userId, Guid appId, Guid planId, string licenseKey, DateTime startDate)
		{
			var activator = CreateActivator();
			// VERIFICĂ: Trimite planId aici?
			activator.Activate(userId, appId, planId, licenseKey, startDate);
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
		// !!! VERIFICĂ ACEASTA LINIE: Trebuie să fie MonthlyActivator !!!
		public override ISubscriptionActivator CreateActivator() => new MonthlyActivator(_repository);
	}

	public class AnnualSubscriptionManager : SubscriptionManager
	{
		public AnnualSubscriptionManager(ISubscriptionRepository repository) : base(repository) { }
		// !!! VERIFICĂ ACEASTA LINIE: Trebuie să fie AnnualActivator !!!
		public override ISubscriptionActivator CreateActivator() => new AnnualActivator(_repository);
	}
}