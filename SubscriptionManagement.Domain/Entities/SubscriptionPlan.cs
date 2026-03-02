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
	}
}
