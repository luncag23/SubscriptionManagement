using DAL.Abstract;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Concrete
{
	public class SubscriptionRepository : ISubscriptionRepository
	{
		private readonly SubscriptionDbContext _context;

		public SubscriptionRepository(SubscriptionDbContext context)
		{
			_context = context;
		}

		// --- PLANURI (Pentru Factory & Prototype) ---
		public async Task<SubscriptionPlan> GetPlanByIdAsync(Guid planId)
		{
			return await _context.SubscriptionPlans.FindAsync(planId);
		}

		public async Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync()
		{
			// Scoatem filtrul .Where(p => p.IsActive) pentru Admin, 
			// ca să poți vedea toate planurile configurate în sistem.
			return await _context.SubscriptionPlans.ToListAsync();
		}

		public async Task AddSubscriptionPlanAsync(SubscriptionPlan plan)
		{
			_context.SubscriptionPlans.Add(plan);
			await _context.SaveChangesAsync();
		}

		// --- UTILIZATORI (Pentru Register & Login) ---
		public async Task<User> GetUserByIdAsync(Guid userId)
		{
			return await _context.Users.FindAsync(userId);
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task AddUserAsync(User user)
		{
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateUserAsync(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		// --- PROFILURI (Pentru Builder) ---
		public async Task AddUserProfileAsync(UserProfile profile)
		{
			_context.UserProfiles.Add(profile);
			await _context.SaveChangesAsync();
		}

		public async Task<UserProfile> GetProfileByUserIdAsync(Guid userId)
		{
			return await _context.UserProfiles
					   .AsNoTracking() // Previne eroarea de tracking la update
					   .FirstOrDefaultAsync(p => p.UserId == userId);
		}

		public async Task UpdateUserProfileAsync(UserProfile profile)
		{
			_context.UserProfiles.Update(profile);
			await _context.SaveChangesAsync();
		}

		// --- SUBSCRIPȚII (Pentru Factory Method) ---
		public async Task SaveSubscriptionAsync(Subscription subscription)
		{
			await _context.Subscriptions.AddAsync(subscription);
			await _context.SaveChangesAsync();
		}

		// --- APLICAȚII ȘI PACHETE (Pentru Composite Pattern) ---
		public async Task AddAppAsync(CreativeApp app)
		{
			_context.CreativeApps.Add(app);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<CreativeApp>> GetAllAppsAsync()
		{
			// Include este vital pentru ca Composite să vadă legăturile Many-to-Many
			return await _context.CreativeApps
				.Include(a => a.AssignmentsAsApp)
				.Include(a => a.AssignmentsAsBundle)
				.ToListAsync();
		}

		public async Task<CreativeApp> GetAppByIdAsync(Guid appId)
		{
			return await _context.CreativeApps.FindAsync(appId);
		}

		public async Task AssignAppToBundleAsync(Guid appId, Guid bundleId)
		{
			var assignment = new AppBundleAssignment { AppId = appId, BundleId = bundleId };
			_context.AppBundleAssignments.Add(assignment);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<CreativeApp>> GetChildrenForBundleAsync(Guid bundleId)
		{
			return await _context.AppBundleAssignments
				.Where(a => a.BundleId == bundleId)
				.Select(a => a.App)
				.ToListAsync();
		}

		public async Task UpdateAppAsync(CreativeApp app)
		{
			_context.CreativeApps.Update(app);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateBundleAssignmentsAsync(Guid bundleId, List<Guid> selectedAppIds)
		{
			// 1. Ștergem toate legăturile actuale ale acestui pachet
			var oldAssignments = _context.AppBundleAssignments.Where(a => a.BundleId == bundleId);
			_context.AppBundleAssignments.RemoveRange(oldAssignments);

			// 2. Adăugăm noile legături bifate în UI
			foreach (var appId in selectedAppIds)
			{
				_context.AppBundleAssignments.Add(new AppBundleAssignment
				{
					BundleId = bundleId,
					AppId = appId
				});
			}

			await _context.SaveChangesAsync();
		}

		public async Task DeleteAppAsync(Guid id)
		{
			var app = await _context.CreativeApps.FindAsync(id);
			if (app != null)
			{
				// 1. Ștergem toate asocierile (unde acest element este fie Părinte, fie Copil)
				var relatedAssignments = _context.AppBundleAssignments
					.Where(a => a.AppId == id || a.BundleId == id);

				_context.AppBundleAssignments.RemoveRange(relatedAssignments);

				// 2. Ștergem aplicația/pachetul propriu-zis
				_context.CreativeApps.Remove(app);

				await _context.SaveChangesAsync();
			}
		}
		public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
		{
			return await _context.Subscriptions
				.Include(s => s.App) // Ne asigurăm că aducem și datele despre produsul cumpărat
				.ToListAsync();
		}

		public async Task<Subscription> GetSubscriptionByIdAsync(Guid id)
		{
			return await _context.Subscriptions.Include(s => s.App).FirstOrDefaultAsync(s => s.Id == id);
		}

		public async Task UpdateSubscriptionAsync(Subscription subscription)
		{
			_context.Subscriptions.Update(subscription);
			await _context.SaveChangesAsync();
		}
	}
}