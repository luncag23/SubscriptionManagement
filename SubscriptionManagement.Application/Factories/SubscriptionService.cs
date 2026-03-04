using System;
using System.Collections.Generic;
using System.Text;
using DAL.Abstract;
using DataContract.DTOs;

namespace BusinessLogic.Factories
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _repository;
		private readonly SubscriptionProvider _provider;

		public SubscriptionService(ISubscriptionRepository repository, SubscriptionProvider provider)
		{
			_repository = repository;
			_provider = provider;
		}

		public async Task<SubscriptionResponse> SubscribeUserAsync(string planType)
		{
			// 1. Luăm datele necesare (folosim ID-urile de test pentru moment)
			var plan = (await _repository.GetAllPlansAsync()).FirstOrDefault();
			var user = await _repository.GetUserByIdAsync(Guid.Parse("22222222-2222-2222-2222-222222222222"));

			if (plan == null || user == null)
				throw new Exception("Datele de test (User/Plan) lipsesc din DB.");

			// 2. Apelăm Factory-ul prin Provider
			var manager = _provider.GetManager(planType);

			// Această metodă execută Factory Method-ul și salvează în DB prin Activator
			manager.ProcessSubscription(user.Id, plan.Id);

			// 3. Mapăm rezultatul către un DTO de răspuns
			return new SubscriptionResponse
			{
				SubscriptionId = Guid.NewGuid(), // În mod real, îl luăm pe cel salvat
				PlanName = plan.Name,
				Status = "Success",
				Message = $"Abonamentul de tip {planType} a fost activat pentru {user.FullName}."
			};
		}
	}
}
