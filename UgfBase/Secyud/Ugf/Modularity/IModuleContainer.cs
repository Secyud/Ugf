#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Modularity
{
	public interface IModuleContainer
	{
		IReadOnlyList<IUgfModuleDescriptor> Modules { get; }
	}
}