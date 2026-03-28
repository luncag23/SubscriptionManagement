using System;
using System.Collections.Generic;

namespace Domain.Entities
{
	public class CreativeApp
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public decimal BasePrice { get; set; }

		// CÂMPURI NOI
		public string? Description { get; set; }
		public string? ImageUrl { get; set; }

		public List<AppBundleAssignment> AssignmentsAsBundle { get; set; } = new();
		public List<AppBundleAssignment> AssignmentsAsApp { get; set; } = new();
	}

	public class AppBundleAssignment
	{
		public Guid BundleId { get; set; } // ID-ul Pachetului (Composite)
		public CreativeApp Bundle { get; set; }

		public Guid AppId { get; set; }    // ID-ul Aplicației incluse (Leaf)
		public CreativeApp App { get; set; }
	}
}