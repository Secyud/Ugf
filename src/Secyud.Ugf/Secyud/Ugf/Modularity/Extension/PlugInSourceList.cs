using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Modularity.Plugins;

namespace Secyud.Ugf.Modularity;

public class PlugInSourceList : List<IPlugInSource>
{
    public IEnumerable<Type> GetAllModules()
    {
        return this.SelectMany(pluginSource => pluginSource.GetModules());
    }
}