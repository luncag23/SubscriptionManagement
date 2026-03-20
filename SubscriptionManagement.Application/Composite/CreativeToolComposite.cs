using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Composite
{
	// 1. COMPONENT - Interfața comună
	public interface ICreativeElement
	{
		string GetName();
		decimal GetPrice();
		void Add(ICreativeElement element); // Doar pentru Composite, dar definit aici pentru uniformitate
	}

	// 2. LEAF - Aplicația simplă (Frunza)
	public class SingleAppLeaf : ICreativeElement
	{
		private readonly string _name;
		private readonly decimal _price;

		public SingleAppLeaf(string name, decimal price)
		{
			_name = name;
			_price = price;
		}

		public string GetName() => _name;
		public decimal GetPrice() => _price;
		public void Add(ICreativeElement element) { /* Frunzele nu pot avea copii */ }
	}

	// 3. COMPOSITE - Pachetul (Grupul)
	public class AppBundleComposite : ICreativeElement
	{
		private readonly string _name;
		private readonly List<ICreativeElement> _children = new();

		public AppBundleComposite(string name) => _name = name;

		public void Add(ICreativeElement element) => _children.Add(element);

		public string GetName()
		{
			// Afișăm numele pachetului și ce conține
			var childrenNames = string.Join(", ", _children.Select(c => c.GetName()));
			return $"{_name} [{childrenNames}]";
		}

		public decimal GetPrice()
		{
			// Calculăm suma tuturor elementelor din interior
			decimal total = _children.Sum(c => c.GetPrice());

			// LOGICĂ DE BUSINESS: Dacă e pachet, oferim 15% reducere automată
			return total * 0.85m;
		}
	}
}