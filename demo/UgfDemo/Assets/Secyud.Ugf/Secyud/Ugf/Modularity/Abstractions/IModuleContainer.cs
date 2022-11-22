using System.Collections.Generic;

namespace Secyud.Ugf.Modularity
{
    public interface IModuleContainer
    {
        IReadOnlyList<IUgfModuleDescriptor> Modules { get; }
    }
}