using DAL.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Proxy
{
	public class AdminService : IAdminService
	{
		private readonly ISubscriptionRepository _repository;

		public AdminService(ISubscriptionRepository repository)
		{
			_repository = repository;
		}

		public async Task<Guid> CreateApp(string name, decimal price, string description, string imageUrl)
		{
			var id = Guid.NewGuid();
			var newApp = new CreativeApp
			{
				Id = id,
				Name = name,
				BasePrice = price,
				Description = description,
				ImageUrl = imageUrl
			};

			await _repository.AddAppAsync(newApp);
			return id;
		}

		public async Task<Guid> CreateBundle(string name, string description)
		{
			var id = Guid.NewGuid();
			var newBundle = new CreativeApp
			{
				Id = id,
				Name = name,
				BasePrice = 0, // Prețul se calculează via Composite
				Description = description,
				ImageUrl = "/images/apps/default-bundle.png" // Imagine default bundle
			};

			await _repository.AddAppAsync(newBundle);
			return id;
		}

		public async Task UpdateApp(CreativeApp app, List<Guid> selectedIds)
		{
			// Actualizăm datele de bază ale aplicației/pachetului
			await _repository.UpdateAppAsync(app);

			// Actualizăm legăturile Many-to-Many (logica Composite)
			if (selectedIds != null)
			{
				await _repository.UpdateBundleAssignmentsAsync(app.Id, selectedIds);
			}
		}
	}
}