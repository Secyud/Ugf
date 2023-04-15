#region

using System;
using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Modularity
{
    public interface IModuleLoader
    {
        IUgfModuleDescriptor[] LoadModules(IDependencyRegistrar registrar,
            Type startupModuleType,
            PlugInSourceList plugInSources);
    }
}