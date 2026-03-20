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
		return accessType.ToLower() switch
		{
			"free" => new FreeTrialManager(_repository),
			"monthly" => new MonthlySubscriptionManager(_repository), // Redenumim Premium în Monthly
			"annual" => new AnnualSubscriptionManager(_repository),  // Adăugăm Annual
			_ => throw new ArgumentException("Tip de acces invalid")
		};
	}
}