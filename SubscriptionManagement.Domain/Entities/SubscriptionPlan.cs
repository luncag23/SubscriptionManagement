using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class SubscriptionPlan
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal MonthlyPrice { get; set; }
		public int MaxStorageGB { get; set; } // Un exemplu de limită/beneficiu
		public bool IsActive { get; set; }

		// --- IMPLEMENTARE PROTOTYPE ---
		public SubscriptionPlan Clone()
		{
			// Cream o copie superficială (Shallow Copy)
			// MemeberwiseClone este o metodă nativă .NET care copiază toate câmpurile
			var clone = (SubscriptionPlan)this.MemberwiseClone();

			// Resetăm ID-ul pentru că noua clonă va fi un rând nou în baza de date
			clone.Id = Guid.NewGuid();

			return clone;
		}
	}
}
