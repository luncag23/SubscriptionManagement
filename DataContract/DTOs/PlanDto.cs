using System;
using System.Collections.Generic;
using System.Text;

namespace DataContract.DTOs
{
	public class PlanDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string StorageInfo { get; set; } // Ex: "20GB Cloud Storage"
	}
}
