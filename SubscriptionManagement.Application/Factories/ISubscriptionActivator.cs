using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Factories
{
	//  Interfata principala
	public interface ISubscriptionActivator
	{
		void Activate(Guid userId, Guid planId, string licenseKey);
	}
}
