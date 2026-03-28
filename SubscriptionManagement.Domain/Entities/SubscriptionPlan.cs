using System;

namespace Domain.Entities
{
	public class SubscriptionPlan
	{
		public Guid Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		// Codul pentru Factory: "free", "monthly", "annual"
		public string PlanTypeCode { get; set; } = string.Empty;

		// Multiplicatorul de preț: 0 (Free), 1 (Monthly), 10 (Annual)
		public decimal PriceMultiplier { get; set; }

		public int MaxStorageGB { get; set; }

		public bool IsActive { get; set; }

		// --- IMPLEMENTARE PROTOTYPE PATTERN ---
		public SubscriptionPlan Clone()
		{
			// MemberwiseClone face o copie superficială a obiectului curent
			return (SubscriptionPlan)this.MemberwiseClone();
		}
	}
}