#region

using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public interface IModuleContainer
    {
        IReadOnlyList<IUgfModuleDescriptor> Modules { get; }
    }
}