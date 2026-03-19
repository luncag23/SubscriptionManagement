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

		public async Task<IEnumerable<SubscriptionPlan>> GetAllPlansAsync()
		{
			return await _context.SubscriptionPlans.Where(p => p.IsActive).ToListAsync();
		}

		public async Task<SubscriptionPlan> GetPlanByIdAsync(Guid planId)
		{
			return await _context.SubscriptionPlans.FindAsync(planId);
		}

		public async Task<User> GetUserByIdAsync(Guid userId)
		{
			return await _context.Users.FindAsync(userId);
		}

		public async Task SaveSubscriptionAsync(Subscription subscription)
		{
			await _context.Subscriptions.AddAsync(subscription);
			await _context.SaveChangesAsync();
		}

		public async Task AddUserAsync(User user)
		{
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
		}

		public async Task AddUserProfileAsync(UserProfile profile)
		{
			_context.UserProfiles.Add(profile);
			await _context.SaveChangesAsync();
		}

		public async Task<User> GetUserByEmailAsync(string email)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		// Implementările corecte (Șterge variantele fără acolade din fișier!)
		public async Task<UserProfile> GetProfileByUserIdAsync(Guid userId)
		{
			return await _context.UserProfiles
					   .AsNoTracking() // <--- ACEASTA ESTE LINIA CRITICĂ
					   .FirstOrDefaultAsync(p => p.UserId == userId);
		}
		public async Task UpdateUserAsync(User user)
		{
			_context.Users.Update(user); // Spunem EF că acest user existent s-a modificat
			await _context.SaveChangesAsync();
		}
		public async Task UpdateUserProfileAsync(UserProfile profile)
		{
			_context.UserProfiles.Update(profile);
			await _context.SaveChangesAsync();
		}

		public async Task AddSubscriptionPlanAsync(SubscriptionPlan plan)
		{
			_context.SubscriptionPlans.Add(plan);
			await _context.SaveChangesAsync();
		}


	}
}