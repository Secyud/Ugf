using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Modularity.Plugins
{
    public interface IPlugInSource
    {
        IEnumerable<Type> GetModules();
    }
}