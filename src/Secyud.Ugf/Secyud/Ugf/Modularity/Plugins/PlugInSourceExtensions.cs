using System;
using System.Linq;

namespace Secyud.Ugf.Modularity.Plugins;

public static class PlugInSourceExtensions
{
    public static Type[] GetModulesWithAllDependencies(this IPlugInSource plugInSource)
    {
        Thrower.IfNull(plugInSource);

        return plugInSource
            .GetModules()
            .SelectMany(UgfModuleHelper.FindAllModuleTypes)
            .Distinct()
            .ToArray();
    }
}