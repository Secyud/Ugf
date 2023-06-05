using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Resource
{
	public class ResourceDescriptor
	{

		private readonly SortedDictionary<int, object> _configs = new();
		
		public string Name { get; }

		public Guid TypeId { get; }
		
		public Type TemplateType { get; }
		
		public object this[int id]
		{
			get => _configs.TryGetValue(id, out object value) ? value : null;
			set => _configs[id] = value;
		}
		public ResourceDescriptor(string name, Guid typeId, Type templateType)
		{
			Name = name;
			TypeId = typeId;
			TemplateType = templateType;
		}

		public TValue Get<TValue>(int id)
		{
			return _configs.TryGetValue(id, out object value) ? (TValue)value : default;
		}
	}
}