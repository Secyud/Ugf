#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Secyud.Ugf.Modularity
{
    internal static class UgfModuleHelper
    {
        public static List<Type> FindAllModuleTypes(Type startupModuleType)
        {
            List<Type> moduleTypes = new List<Type>();
            AddModuleAndDependenciesRecursively(moduleTypes, startupModuleType);
            return moduleTypes;
        }

        private static void AddModuleAndDependenciesRecursively(
            ICollection<Type> moduleTypes,
            Type moduleType)
        {
            UgfExtension.CheckUgfModuleType(moduleType);

            if (moduleTypes.Contains(moduleType)) return;

            moduleTypes.Add(moduleType);

            foreach (Type dependedModuleType in FindDependedModuleTypes(moduleType))
                AddModuleAndDependenciesRecursively(moduleTypes, dependedModuleType);
        }

        public static List<Type> FindDependedModuleTypes(Type moduleType)
        {
            UgfExtension.CheckUgfModuleType(moduleType);

            DependsOnAttribute depends =
                moduleType.GetCustomAttribute<DependsOnAttribute>() ?? new DependsOnAttribute();

            List<Type> dependencies = depends.DependedTypes.Distinct().ToList();

            return dependencies;
        }
    }
}