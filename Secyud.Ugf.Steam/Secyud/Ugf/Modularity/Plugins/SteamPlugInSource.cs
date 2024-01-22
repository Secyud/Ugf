using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.Logging;
using Secyud.Ugf.Steam.WorkshopManager;

namespace Secyud.Ugf.Modularity.Plugins
{
    public class SteamPlugInSource : IPlugInSource
    {
        private static List<Type> _loadedModules;

        private static void LoadModules()
        {
            _loadedModules = new List<Type>();
            foreach (WorkshopItemInfo item in LocalWorkshopManager.LocalItems)
            {
                if (!item.Available || item.ConfigInfo.Disabled) continue;

                string moduleAssemblyName = Path.Combine(
                    item.LocalPath,
                    item.ConfigInfo.Assembly);

                if (!File.Exists(moduleAssemblyName)) continue;

                Assembly assembly = Assembly.LoadFrom(moduleAssemblyName);
                Type type = assembly.ExportedTypes.FirstOrDefault(
                    u => typeof(IUgfModule).IsAssignableFrom(u));

                if (type is null)
                {
                    UgfLogger.LogWarning(
                        $"Assembly {assembly} doesn't contains a valid" +
                        " ugf module. It is loaded as environment only!");
                }
                else
                {
                    _loadedModules.Add(type);
                }
            }
        }

        public IEnumerable<Type> GetModules()
        {
            if (_loadedModules is null)
            {
                LoadModules();
            }

            return _loadedModules;
        }
    }
}