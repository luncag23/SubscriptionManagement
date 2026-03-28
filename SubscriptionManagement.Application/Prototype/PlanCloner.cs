using DAL.Abstract;
using Domain.Entities;

namespace BusinessLogic.Prototype
{
	public class PlanCloner
	{
		private readonly ISubscriptionRepository _repository;

		public PlanCloner(ISubscriptionRepository repository)
		{
			_repository = repository;
		}

		public async Task<SubscriptionPlan> CreatePlanFromTemplate(Guid templatePlanId, string newName, decimal newMultiplier)
		{
			var templatePlan = await _repository.GetPlanByIdAsync(templatePlanId);
			if (templatePlan == null) throw new Exception("Planul template nu a fost găsit.");

			// Pattern-ul Prototype
			var newPlan = templatePlan.Clone();

			newPlan.Name = newName;
			newPlan.PriceMultiplier = newMultiplier; // Actualizăm multiplicatorul
			newPlan.Description = $"[CLONĂ] {templatePlan.Description}";

			await _repository.AddSubscriptionPlanAsync(newPlan);
			return newPlan;
		}
	}
}