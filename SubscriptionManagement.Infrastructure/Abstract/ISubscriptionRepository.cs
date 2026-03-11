using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Abstract
{
	public interface ISubscriptionRepository
	{
		Task<SubscriptionPlan> GetPlanByIdAsync(Guid planId);
		Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync();
		Task<User> GetUserByIdAsync(Guid userId);
		Task SaveSubscriptionAsync(Subscription subscription);

		Task AddUserAsync(User user);
		Task AddUserProfileAsync(UserProfile profile);
		Task<User> GetUserByEmailAsync(string email);

		// ASIGURĂ-TE CĂ ACESTE DOUĂ SUNT AICI:
		Task<UserProfile> GetProfileByUserIdAsync(Guid userId);
		Task UpdateUserAsync(User user);
		Task AddSubscriptionPlanAsync(SubscriptionPlan plan);
		Task UpdateUserProfileAsync(UserProfile profile);
	}
}