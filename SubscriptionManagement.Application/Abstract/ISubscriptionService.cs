using System;
using System.Collections.Generic;
using System.Text;
using DataContract.DTOs;

namespace BusinessLogic.Abstract
{
	public interface ISubscriptionService
	{
		// Actiunea principala: Procesarea unei abonări
		Task<SubscriptionResponse> SubscribeUserAsync(CreateSubscriptionRequest request);

		// Obținerea listei de planuri pentru UI
		Task<IEnumerable<PlanDto>> GetAvailablePlansAsync();
	}
}
