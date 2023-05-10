#region

using System;

#endregion

namespace Secyud.Ugf.Modularity
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DependsOnAttribute : Attribute, IDependedTypesProvider
	{
		public DependsOnAttribute(params Type[] dependedTypes)
		{
			DependedTypes = dependedTypes ?? Type.EmptyTypes;
		}

		public Type[] DependedTypes { get; }
	}
}