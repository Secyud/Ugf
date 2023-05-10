#region

using System;

#endregion

namespace Secyud.Ugf.Modularity
{
	public interface IDependedTypesProvider
	{
		Type[] DependedTypes { get; }
	}
}