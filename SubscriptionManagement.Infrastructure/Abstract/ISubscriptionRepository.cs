using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace DAL.Abstract
{
	public interface ISubscriptionRepository
	{
		// Operatii de baza pe baza de date (CRUD)
		Task<SubscriptionPlan> GetPlanByIdAsync(Guid planId);
		Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync();
		Task<User> GetUserByIdAsync(Guid userId);
		Task SaveSubscriptionAsync(Subscription subscription);
	}
}
