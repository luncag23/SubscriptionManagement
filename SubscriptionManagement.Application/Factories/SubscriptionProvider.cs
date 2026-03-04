using System;
using System.Collections.Generic;
using System.Text;
using DAL.Abstract;

namespace BusinessLogic.Factories
{
	public class SubscriptionProvider
	{
		private readonly ISubscriptionRepository _repository;

		public SubscriptionProvider(ISubscriptionRepository repository)
		{
			_repository = repository;
		}

		public SubscriptionManager GetManager(string type)
		{
			return type.ToLower() switch
			{
				"free" => new FreeTrialManager(_repository),
				"premium" => new PremiumSubscriptionManager(_repository),
				_ => throw new ArgumentException("Invalid type")
			};
		}
	}

}
