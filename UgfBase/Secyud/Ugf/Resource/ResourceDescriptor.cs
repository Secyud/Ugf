using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Resource
{
	public class ResourceDescriptor
	{

		private readonly SortedDictionary<int, object> _configs = new();

		public string Name => Get<string>(-1);

		public Guid TypeId => Get<Guid>(-2);
		
		public object this[int id]
		{
			get => _configs.TryGetValue(id, out object value) ? value : null;
			set => _configs[id] = value;
		}

		public TValue Get<TValue>(int id)
		{
			return _configs.TryGetValue(id, out object value) ? (TValue)value : default;
		}
	}
}