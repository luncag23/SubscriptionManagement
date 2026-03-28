using BusinessLogic.Factories;
using DAL.Abstract;

public class SubscriptionProvider
{
	private readonly ISubscriptionRepository _repository;

	public SubscriptionProvider(ISubscriptionRepository repository)
	{
		_repository = repository;
	}

	public SubscriptionManager GetManager(string accessType)
	{
		return accessType.ToLower().Trim() switch
		{
			"free" => new FreeTrialManager(_repository),
			"monthly" => new MonthlySubscriptionManager(_repository),
			"annual" => new AnnualSubscriptionManager(_repository),
			_ => new MonthlySubscriptionManager(_repository) // Fallback sigur pe Monthly
		};
	}
}