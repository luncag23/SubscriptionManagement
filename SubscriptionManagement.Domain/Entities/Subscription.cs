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

		// SCHIMBĂM AICI: În loc de PlanId, folosim AppId (care poate fi App sau Bundle)
		public Guid AppId { get; set; }
		public Guid PlanId { get; set; }
		public CreativeApp App { get; set; } // Referință către Composite element

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string LicenseKey { get; set; }
		public DataContract.Enums.SubscriptionStatus Status { get; set; }

		public User User { get; set; }
	}
}
