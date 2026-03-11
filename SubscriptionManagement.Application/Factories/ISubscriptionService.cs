using System;
using System.Collections.Generic;
using System.Text;
using DataContract.DTOs;

namespace BusinessLogic.Factories
{
	public interface ISubscriptionService
	{
		// Adăugăm Guid userId la final
		Task<SubscriptionResponse> SubscribeUserAsync(string planType, string paymentType, Guid userId);
	}
}
