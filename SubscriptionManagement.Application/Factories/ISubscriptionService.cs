using System;
using System.Collections.Generic;
using System.Text;
using DataContract.DTOs;

namespace BusinessLogic.Factories
{
	public interface ISubscriptionService
	{
		// Adăugăm Guid userId la final
		Task<SubscriptionResponse> SubscribeUserAsync(Guid appId, string accessType, string paymentType, Guid userId);
	}
}
