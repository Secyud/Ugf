using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.Modularity.Plugins
{
    public class FilePluginSource : TypePlugInSource
    {
        public FilePluginSource(params string[] filePaths)
        {
            ModuleTypes = filePaths
                .Select(Assembly.LoadFrom)
                .Select(assembly => assembly.ExportedTypes
                    .First(u => typeof(IUgfModule).IsAssignableFrom(u)))
                .Where(type => type is not null).ToArray();
        }
    }
}