using System;
using System.Collections.Generic;
using System.Text;
using DataContract.Enums;

namespace Domain.Entities
{
	public class Subscription
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid PlanId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public SubscriptionStatus Status { get; set; }

		// Referințe de navigare (utile pentru logica internă)
		public User User { get; set; }
		public SubscriptionPlan Plan { get; set; }
	}
}
