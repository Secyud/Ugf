using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Steamworks;

namespace Secyud.Ugf.Modularity.Plugins
{
    public class SteamPlugInSource: TypePlugInSource
    {
        public SteamPlugInSource()
        {
            ModuleTypes = GetSteamWorkPlugins();
        }

        private Type[] GetSteamWorkPlugins()
        {
            uint itemNum = SteamUGC.GetNumSubscribedItems();
            PublishedFileId_t[] fileIds = new PublishedFileId_t[itemNum];
            itemNum = SteamUGC.GetSubscribedItems(fileIds, itemNum);

            List<Type> pluginTypes = new();

            foreach (PublishedFileId_t fileId in fileIds)
            {
                if (SteamUGC.GetItemInstallInfo(fileId, 
                        out ulong size, out string folder, 
                        260, out uint stamp))
                {
                    string[] files = Directory.GetFiles(folder);

                    foreach (string file in files.Where(u => u.EndsWith(".dll")))
                    {
                        Assembly assembly = Assembly.LoadFile(file);
                        Type type = assembly.ExportedTypes.FirstOrDefault(
                            u => typeof(IUgfModule).IsAssignableFrom(u));
                        if (type is not null)
                        {
                            pluginTypes.Add(type);
                        }
                    }
                }
            }
            
            return pluginTypes.ToArray();
        }
    }
}