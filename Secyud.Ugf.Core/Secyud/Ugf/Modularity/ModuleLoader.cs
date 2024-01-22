using System;
using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Modularity
{
    public class ModuleLoader
    {
        public static List<IUgfModuleDescriptor> LoadModules(
            Type startupModuleType,
            PlugInSourceList plugInSources)
        {
            Throw.IfNull(startupModuleType);

            List<IUgfModuleDescriptor> modules = GetDescriptors(startupModuleType, plugInSources);

            modules = SortByDependency(modules, startupModuleType);

            return modules;
        }

        private static List<IUgfModuleDescriptor> GetDescriptors(
            Type startupModuleType,
            PlugInSourceList plugInSources)
        {
            List<UgfModuleDescriptor> modules = GetModules(startupModuleType, plugInSources);

            foreach (UgfModuleDescriptor module in modules)
            {
                foreach (Type dependedModuleType in DependsOnAttribute.GetDependModules(module.Type))
                {
                    UgfModuleDescriptor dependedModule = modules.FirstOrDefault(m => m.Type == dependedModuleType);

                    module.AddDependency(dependedModule);
                }
            }

            return modules.Cast<IUgfModuleDescriptor>().ToList();
        }

        private static List<UgfModuleDescriptor> GetModules(
            Type startupModuleType, PlugInSourceList plugInSources)
        {
            // startup module
            List<UgfModuleDescriptor> descriptors = new() { CreateDescriptor(startupModuleType) };
            // ! descriptors.Count is changed in loop
            for (int i = 0; i < descriptors.Count; i++)
            {
                UgfModuleDescriptor moduleDescriptor = descriptors[i];
                Type[] dependModules = DependsOnAttribute.GetDependModules(moduleDescriptor.Type);
                TryAddDescriptors(descriptors, dependModules);
            }

            //Plugin modules
            if (plugInSources is not null)
            {
                TryAddDescriptors(descriptors,
                    plugInSources.GetAllModules());
            }

            return descriptors;
        }


        private static List<IUgfModuleDescriptor> SortByDependency(IEnumerable<IUgfModuleDescriptor> modules,
            Type startupModuleType)
        {
            List<IUgfModuleDescriptor> sortedModules = SortByDependencies(modules, m => m.Dependencies);

            int currentIndex = sortedModules.FindIndex(u => u.Type == startupModuleType);
            if (currentIndex != sortedModules.Count - 1)
            {
                IUgfModuleDescriptor item = sortedModules[currentIndex];
                sortedModules.RemoveAt(currentIndex);
                sortedModules.Add(item);
            }

            return sortedModules;
        }

        private static UgfModuleDescriptor CreateDescriptor(Type moduleType, bool isLoadedAsPlugIn = false)
        {
            if (moduleType.IsAbstract || moduleType.IsGenericType || !moduleType.IsClass ||
                !typeof(IUgfModule).IsAssignableFrom(moduleType))
            {
                throw new ArgumentException(
                    "Given type is not an UGF module: "
                    + moduleType.AssemblyQualifiedName
                );
            }

            IUgfModule instance = (IUgfModule)Activator.CreateInstance(moduleType);
            UgfModuleDescriptor descriptor = new(moduleType, instance, isLoadedAsPlugIn);
            return descriptor;
        }

        private static void TryAddDescriptors(
            ICollection<UgfModuleDescriptor> descriptors,
            IEnumerable<Type> moduleTypes,
            bool isLoadedAsPlugIn = false)
        {
            foreach (Type moduleType in moduleTypes)
            {
                if (descriptors.All(m => m.Type != moduleType))
                {
                    descriptors.Add(CreateDescriptor(moduleType, isLoadedAsPlugIn));
                }
            }
        }

        /// <summary>
        ///     Sort a list by a topological sorting, which consider their dependencies.
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="source">A list of objects to sort</param>
        /// <param name="getDependencies">Function to resolve the dependencies</param>
        /// <param name="comparer">Equality comparer for dependencies </param>
        /// <returns>
        ///     Returns a new list ordered by dependencies.
        ///     If A depends on B, then B will come before than A in the resulting list.
        /// </returns>
        public static List<T> SortByDependencies<T>(
            IEnumerable<T> source,
            Func<T, IEnumerable<T>> getDependencies,
            IEqualityComparer<T> comparer = null)
        {
            // See: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
            //      http://en.wikipedia.org/wiki/Topological_sorting

            List<T> sorted = new();
            Dictionary<T, bool> visited = new(comparer);

            foreach (T item in source)
            {
                SortByDependenciesVisit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="item">Item to resolve</param>
        /// <param name="getDependencies">Function to resolve the dependencies</param>
        /// <param name="sorted">List with the sorted items</param>
        /// <param name="visited">Dictionary with the visited items</param>
        private static void SortByDependenciesVisit<T>(
            T item,
            Func<T, IEnumerable<T>> getDependencies,
            ICollection<T> sorted,
            IDictionary<T, bool> visited)
        {
            bool alreadyVisited = visited.TryGetValue(item, out bool inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
            }
            else
            {
                visited[item] = true;

                IEnumerable<T> dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (T dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}