using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Collections
{
	public class RegistrableList<TItem>
	{
		private readonly List<TItem> _items = new();

		public void Register(TItem item)
		{
			_items.Add(item);
		}

		public void RegisterList(params TItem[] items)
		{
			foreach (var item in items) Register(item);
		}

		public List<TItem> Get()
		{
			return _items.ToList();
		}
	}
}