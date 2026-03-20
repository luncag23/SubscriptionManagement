using System;
using System.Collections.Generic;

namespace Domain.Entities
{
	public class CreativeApp
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public decimal BasePrice { get; set; }

		// O aplicație poate fi parte din mai multe pachete (Assignments ca 'App')
		// Un pachet poate conține mai multe aplicații (Assignments ca 'Bundle')
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