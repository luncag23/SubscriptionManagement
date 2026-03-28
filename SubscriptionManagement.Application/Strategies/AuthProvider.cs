using System.Collections.Generic;
using DAL.Abstract;

namespace BusinessLogic.Strategies
{
	public class AuthProvider
	{
		private readonly Dictionary<string, IAuthStrategy> _strategies = new();

		public AuthProvider(ISubscriptionRepository repo)
		{
			_strategies["local"] = new LocalAuthStrategy(repo);
			_strategies["google"] = new GoogleAuthStrategy();
			
		}

		public IAuthStrategy GetStrategy(string provider)
		{
			return _strategies.ContainsKey(provider.ToLower())
				   ? _strategies[provider.ToLower()]
				   : _strategies["local"];
		}
	}
}