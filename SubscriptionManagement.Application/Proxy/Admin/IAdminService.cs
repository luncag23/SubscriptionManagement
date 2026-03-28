using Domain.Entities;

namespace BusinessLogic.Proxy
{
	public interface IAdminService
	{
		// Adăugăm description și imageUrl
		Task<Guid> CreateApp(string name, decimal price, string description, string imageUrl);
		Task<Guid> CreateBundle(string name, string description);
		Task UpdateApp(CreativeApp app, List<Guid> selectedIds);
	}
}