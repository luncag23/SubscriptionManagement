using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Configurations
{
	public class SubscriptionDbContext : DbContext
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configurații Fluent API (SOLID: Separăm regulile de mapare)
			modelBuilder.Entity<Subscription>()
				.HasOne(s => s.User)
				.WithMany(u => u.Subscriptions)
				.HasForeignKey(s => s.UserId);

			modelBuilder.Entity<Subscription>()
				.HasOne(s => s.Plan)
				.WithMany()
				.HasForeignKey(s => s.PlanId);

			base.OnModelCreating(modelBuilder);
		}
	}
}
