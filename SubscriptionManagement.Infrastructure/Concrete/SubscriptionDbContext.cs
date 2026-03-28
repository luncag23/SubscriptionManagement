using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Concrete
{
	public class SubscriptionDbContext : DbContext
	{
		public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : base(options) { }

		// 1. TABELELE PRINCIPALE
		public DbSet<User> Users { get; set; }
		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
		public DbSet<Subscription> Subscriptions { get; set; }
		public DbSet<CreativeApp> CreativeApps { get; set; }

		// Tabelul de legătură pentru Composite Pattern (Many-to-Many)
		public DbSet<AppBundleAssignment> AppBundleAssignments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// --- A. CONFIGURARE SUBSCRIPTIONS (Legătura cu Composite) ---
			modelBuilder.Entity<Subscription>()
			   .HasOne(s => s.App)
			   .WithMany()
			   .HasForeignKey(s => s.AppId) // <--- Verifică să fie AppId aici
			   .OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Subscription>()
			   .HasOne(s => s.App)
			   .WithMany()
			   .HasForeignKey(s => s.AppId)
			   .OnDelete(DeleteBehavior.Restrict);

			// --- B. REGLARE PRECIZIE DECIMAL (Bani și Multiplicatori) ---
			// Multiplicatorul poate avea zecimale (ex: 0.5 pentru reducere 50%)
			modelBuilder.Entity<SubscriptionPlan>()
			    .Property(p => p.PriceMultiplier).HasColumnType("decimal(18,2)");

			modelBuilder.Entity<CreativeApp>()
			    .Property(p => p.BasePrice).HasColumnType("decimal(18,2)");

			// --- C. LOGICA COMPOSITE (Many-to-Many Self-Reference) ---
			// Această parte a fost cea mai problematică și trebuie scrisă EXACT așa:

			modelBuilder.Entity<AppBundleAssignment>()
			    .HasKey(a => new { a.BundleId, a.AppId }); // Cheie compusă

			modelBuilder.Entity<AppBundleAssignment>()
			    .HasOne(a => a.Bundle)
			    .WithMany(b => b.AssignmentsAsBundle)
			    .HasForeignKey(a => a.BundleId)
			    .OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<AppBundleAssignment>()
			    .HasOne(a => a.App)
			    .WithMany(p => p.AssignmentsAsApp)
			    .HasForeignKey(a => a.AppId)
			    .OnDelete(DeleteBehavior.Restrict);

			base.OnModelCreating(modelBuilder);
		}
	}
}