using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Secyud.Ugf.Modularity.Plugins;

public class FolderPlugInSource:IPlugInSource
{
    public string Folder { get; }

    public SearchOption SearchOption { get; set; }

    public Func<string, bool> Filter { get; set; }

    public FolderPlugInSource(string folder, Func<string, bool> filter, SearchOption searchOption)
    {
        Thrower.IfNull(folder);
        
        Folder = folder;
        Filter = filter;
        SearchOption = searchOption;
    }
    
    public IEnumerable<Type> GetModules()
    {  
        var modules = new List<Type>();

        foreach (var assembly in GetAssemblies())
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                    if (UgfModule.IsUgfModule(type))
                        modules.AddIfNotContains(type);
            }
            catch (Exception ex)
            {
                throw new UgfException("Could not get module types from assembly: " + assembly.FullName, ex);
            }
        }

        return modules.ToArray();
    }
    
    private List<Assembly> GetAssemblies()
    {
        var assemblyFiles = Directory
            .EnumerateFiles(Folder, "*.*", SearchOption)
            .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));

        if (Filter != null)
            assemblyFiles = assemblyFiles.Where(Filter);

        return assemblyFiles.Select(AssemblyLoadContext.Default.LoadFromAssemblyPath).ToList();
    }
}