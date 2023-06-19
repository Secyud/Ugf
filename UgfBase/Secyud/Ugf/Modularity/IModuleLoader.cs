#region

using Secyud.Ugf.DependencyInjection;
using System;

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