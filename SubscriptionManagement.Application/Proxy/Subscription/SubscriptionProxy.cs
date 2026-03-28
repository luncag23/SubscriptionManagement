using BusinessLogic.Factories;
using DAL.Abstract;
using DataContract.DTOs;

namespace BusinessLogic.Proxy
{
	public class SubscriptionProxy : ISubscriptionService
	{
		private readonly SubscriptionService _realService;
		private readonly ISubscriptionRepository _repository;

		public SubscriptionProxy(SubscriptionService realService, ISubscriptionRepository repository)
		{
			_realService = realService;
			_repository = repository;
		}

		public async Task<SubscriptionResponse> SubscribeUserAsync(Guid appId, Guid planId, string planTypeCode, decimal multiplier, string paymentType, Guid userId)
		{
			var allSubs = await _repository.GetAllSubscriptionsAsync();
			var existingActive = allSubs.FirstOrDefault(s => s.UserId == userId && s.AppId == appId && s.EndDate > DateTime.Now);

			// Blocăm doar dacă userul are deja un plan plătit și încearcă să mai ia un FREE TRIAL
			if (existingActive != null && planTypeCode == "free")
			{
				return new SubscriptionResponse { Message = "Blocat de Proxy|Ai deja un abonament activ. Nu poți folosi un Trial acum." };
			}

			// Permitem orice altă combinație (Upgrade de la Trial la Monthly, sau prelungire Monthly)
			return await _realService.SubscribeUserAsync(appId, planId, planTypeCode, multiplier, paymentType, userId);
		}

		public async Task CancelSubscriptionAsync(Guid subscriptionId)
		{
			// Proxy-ul doar deleagă apelul către serviciul real
			// (Aici ai putea adăuga verificări extra de securitate dacă vrei)
			await _realService.CancelSubscriptionAsync(subscriptionId);
		}
	}
}