using System;
using System.Collections.Generic;
using System.Text;
using DAL.Abstract;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;


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
			return await _context.SubscriptionPlans
								 .Where(p => p.IsActive)
								 .ToListAsync();
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
			await _context.SaveChangesAsync(); // Salvează modificările în DB
		}
	}
}
