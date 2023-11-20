using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.Modularity.Plugins
{
    public class FilePluginSource : TypePlugInSource
    {
        public FilePluginSource(params string[] filePaths)
        {
            ModuleTypes = filePaths
                .Select(Assembly.LoadFile)
                .Select(assembly => assembly.ExportedTypes
                    .First(u => typeof(IUgfModule).IsAssignableFrom(u)))
                .Where(type => type is not null).ToArray();
        }
    }
}