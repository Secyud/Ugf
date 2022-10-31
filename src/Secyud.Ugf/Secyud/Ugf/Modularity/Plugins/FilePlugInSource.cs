using System;
using System.Collections.Generic;
using System.Runtime.Loader;

namespace Secyud.Ugf.Modularity.Plugins;

public class FilePlugInSource:IPlugInSource
{
    public string[] FilePaths { get; }
    
    public FilePlugInSource(params string[] filePaths)
    {
        FilePaths = filePaths ?? Array.Empty<string>();
    }
    
    public IEnumerable<Type> GetModules()
    {
        var modules = new List<Type>();

        foreach (var filePath in FilePaths)
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);

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
}