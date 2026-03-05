using System;
using System.Collections.Generic;
using System.Text;
using DataContract.DTOs;

namespace BusinessLogic.Factories
{
	public interface ISubscriptionService
	{
		// Aceasta este metoda principală pe care o va apela UI-ul
		Task<SubscriptionResponse> SubscribeUserAsync(string planType, string paymentType);

	}
}
