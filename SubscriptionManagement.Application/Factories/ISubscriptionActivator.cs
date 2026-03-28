using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Factories
{
	public interface ISubscriptionActivator
	{
		void Activate(Guid userId, Guid appId, Guid planId, string licenseKey, DateTime startDate);
	}
}
