using DataContract.DTOs;

namespace BusinessLogic.Factories
{
	public interface ISubscriptionService
	{

		// Trebuie să aibă și ea 'Guid planId'
		Task<SubscriptionResponse> SubscribeUserAsync(Guid appId, Guid planId, string planTypeCode, decimal multiplier, string paymentType, Guid userId);
		Task CancelSubscriptionAsync(Guid subscriptionId);
	}
}