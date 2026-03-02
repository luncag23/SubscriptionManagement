using System;
using System.Collections.Generic;
using System.Text;

namespace DataContract.DTOs
{
	public class CreateSubscriptionRequest
	{
		public Guid UserId { get; set; }
		public Guid PlanId { get; set; }

		// Putem adăuga detalii de plată aici mai târziu (ex: Token de la Stripe/PayPal)
		public string PaymentMethod { get; set; }
	}
}
