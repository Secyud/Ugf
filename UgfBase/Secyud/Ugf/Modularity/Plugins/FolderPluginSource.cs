using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.Modularity.Plugins
{
    public class FolderPluginSource : TypePlugInSource
    {
        public FolderPluginSource(string folderPath)
        {
            List<Type> pluginTypes = new();

            CheckTargetFolder(folderPath);

            ModuleTypes = pluginTypes.ToArray();
            
            return;


            void CheckTargetFolder(string folder)
            {
                if (!Directory.Exists(folder))
                    return;
                
                string[] files = Directory.GetFiles(folder);

                foreach (string file in files.Where(u => u.EndsWith(".dll")))
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(file);
                        Type type = assembly.ExportedTypes.FirstOrDefault(
                            u => typeof(IUgfModule).IsAssignableFrom(u));
                        if (type is not null)
                        {
                            pluginTypes.Add(type);
                        }
                    }
                    catch (Exception e)
                    {
                        U.LogError(e);
                    }
                }

                string[] subFolders = Directory.GetDirectories(folder);

                foreach (string subFolder in subFolders)
                {
                    CheckTargetFolder(subFolder);
                }
            }
        }
    }
}