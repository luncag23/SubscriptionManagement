using System;
using System.Collections.Generic;

namespace BusinessLogic.AbstractFactory
{
	public class BillingProvider
	{
		private readonly Dictionary<string, IBillingFactory> _factories = new();

		public BillingProvider()
		{
			_factories["google"] = new GooglePayFactory();
			_factories["paypal"] = new PayPalFactory();
			_factories["card"] = new CreditCardFactory();
		}

		public IBillingFactory GetFactory(string type)
		{
			if (!_factories.ContainsKey(type.ToLower()))
				throw new ArgumentException("Metodă de plată nesuportată!");

			return _factories[type.ToLower()];
		}
	}
}