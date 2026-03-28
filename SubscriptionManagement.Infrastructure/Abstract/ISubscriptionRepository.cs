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

		// --- METODE NOI PENTRU COMPOSITE MANY-TO-MANY ---
		Task AddAppAsync(CreativeApp app);
		Task<IEnumerable<CreativeApp>> GetAllAppsAsync();
		Task<CreativeApp> GetAppByIdAsync(Guid appId);
		Task AssignAppToBundleAsync(Guid appId, Guid bundleId);
		Task<IEnumerable<CreativeApp>> GetChildrenForBundleAsync(Guid bundleId);
		Task UpdateAppAsync(CreativeApp app);
		Task UpdateBundleAssignmentsAsync(Guid bundleId, List<Guid> selectedAppIds);
		Task DeleteAppAsync(Guid id);
		Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();

		Task<Subscription> GetSubscriptionByIdAsync(Guid id);
		Task UpdateSubscriptionAsync(Subscription subscription);

	}
}