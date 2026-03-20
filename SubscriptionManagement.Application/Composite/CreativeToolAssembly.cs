using DAL.Abstract;
using Domain.Entities;

namespace BusinessLogic.Composite
{
	public class CreativeToolAssembly
	{
		private readonly ISubscriptionRepository _repository;

		public CreativeToolAssembly(ISubscriptionRepository repository)
		{
			_repository = repository;
		}

		// Metoda care construiește arborele recursiv
		public async Task<ICreativeElement> LoadToolHierarchy(Guid appId)
		{
			var appEntity = await _repository.GetAppByIdAsync(appId);
			if (appEntity == null) return null;

			// Verificăm dacă acest element are "copii" în tabelul de legătură
			var children = await _repository.GetChildrenForBundleAsync(appId);

			if (!children.Any())
			{
				// E o aplicație simplă
				return new SingleAppLeaf(appEntity.Name, appEntity.BasePrice);
			}
			else
			{
				// E un pachet
				var composite = new AppBundleComposite(appEntity.Name);
				foreach (var child in children)
				{
					// Apel recursiv
					composite.Add(await LoadToolHierarchy(child.Id));
				}
				return composite;
			}
		}
	}
}