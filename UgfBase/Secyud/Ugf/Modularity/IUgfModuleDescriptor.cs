#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public interface IUgfModuleDescriptor
    {
        Type Type { get; }

        Assembly Assembly { get; }

        IUgfModule Instance { get; }

        bool IsLoadedAsPlugIn { get; }

        IReadOnlyList<IUgfModuleDescriptor> Dependencies { get; }
    }
}