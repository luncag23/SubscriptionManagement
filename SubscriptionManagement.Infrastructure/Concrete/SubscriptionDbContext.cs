using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Concrete
{
	public class SubscriptionDbContext : DbContext
	{
		public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
		public DbSet<Subscription> Subscriptions { get; set; }
		public DbSet<CreativeApp> CreativeApps { get; set; }
		public DbSet<AppBundleAssignment> AppBundleAssignments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// --- Configurații de bază ---
			modelBuilder.Entity<Subscription>()
				.HasOne(s => s.User).WithMany(u => u.Subscriptions).HasForeignKey(s => s.UserId);

			modelBuilder.Entity<Subscription>()
			   .HasOne(s => s.App)
			   .WithMany() // O aplicație poate apărea în mai multe abonamente
			   .HasForeignKey(s => s.AppId);

			modelBuilder.Entity<SubscriptionPlan>()
				.Property(p => p.MonthlyPrice).HasColumnType("decimal(18,2)");

			modelBuilder.Entity<CreativeApp>()
				.Property(p => p.BasePrice).HasColumnType("decimal(18,2)");

			// --- Logica de Many-to-Many (Mutată din Configurator) ---
			modelBuilder.Entity<AppBundleAssignment>()
				.HasKey(a => new { a.BundleId, a.AppId });

			modelBuilder.Entity<AppBundleAssignment>()
				.HasOne(a => a.Bundle).WithMany(b => b.AssignmentsAsBundle)
				.HasForeignKey(a => a.BundleId).OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<AppBundleAssignment>()
				.HasOne(a => a.App).WithMany(p => p.AssignmentsAsApp)
				.HasForeignKey(a => a.AppId).OnDelete(DeleteBehavior.Restrict);

			base.OnModelCreating(modelBuilder);
		}
	}
}