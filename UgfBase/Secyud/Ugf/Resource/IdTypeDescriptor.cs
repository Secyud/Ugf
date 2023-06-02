using System;

namespace Secyud.Ugf.Resource
{
	public class IdTypeDescriptor<TType> : IHasId<string>
	{
		private IdTypeDescriptor(string id, Type type)
		{
			Id = id;
			Type = type;
		}

		public string Id { get; }

		public Type Type { get; }

		public static IdTypeDescriptor<TType> Describe<TObject>(string prefix = null)
			where TObject : ResourcedBase,TType
		{
			Type type = typeof(TObject);

			return new IdTypeDescriptor<TType>(prefix + "." + type.Name, type);
		}
	}
}