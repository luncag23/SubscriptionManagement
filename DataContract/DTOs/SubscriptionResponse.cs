using System;
using System.Collections.Generic;
using System.Text;

namespace DataContract.DTOs
{
	public class SubscriptionResponse
	{
		public Guid SubscriptionId { get; set; }
		public string PlanName { get; set; }
		public string Status { get; set; } // "Active", "Trial", etc.
		public DateTime ExpirationDate { get; set; }
		public string Message { get; set; } // "Abonamentul tău expiră în 30 de zile"
	}
}
