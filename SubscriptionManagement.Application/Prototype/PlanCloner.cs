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

		public async Task<SubscriptionPlan> CreatePlanFromTemplate(Guid templatePlanId, string newName, decimal newPrice)
		{
			// 1. Luăm planul existent (Prototipul)
			var templatePlan = await _repository.GetPlanByIdAsync(templatePlanId);

			if (templatePlan == null) throw new Exception("Planul template nu a fost găsit.");

			// 2. FOLOSIM PATTERN-UL PROTOTYPE
			var newPlan = templatePlan.Clone();

			// 3. Modificăm doar ce este specific pentru oferta nouă
			newPlan.Name = newName;
			newPlan.MonthlyPrice = newPrice;
			newPlan.Description = $"[CLONĂ] {templatePlan.Description}";

			// 4. Salvăm noul plan în baza de date (avem nevoie de o metodă în Repo)
			await _repository.AddSubscriptionPlanAsync(newPlan);

			return newPlan;
		}
	}
}