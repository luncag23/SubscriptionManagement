using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
	public class User
	{
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string FullName { get; set; }
		public DateTime JoinedDate { get; set; }
		// Putem avea o listă de abonamente istorice sau active
		public List<Subscription> Subscriptions { get; set; } = new();
	}
}
