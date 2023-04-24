#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Modularity.Plugins
{
    public interface IPlugInSource
    {
        IEnumerable<Type> GetModules();
    }
}