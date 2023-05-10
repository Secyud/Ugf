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
			var moduleTypes = new List<Type>();
			AddModuleAndDependenciesRecursively(moduleTypes, startupModuleType);
			return moduleTypes;
		}

		private static void AddModuleAndDependenciesRecursively(
			ICollection<Type> moduleTypes,
			Type moduleType)
		{
			UgfExtensions.CheckUgfModuleType(moduleType);

			if (moduleTypes.Contains(moduleType)) return;

			moduleTypes.Add(moduleType);

			foreach (var dependedModuleType in FindDependedModuleTypes(moduleType))
				AddModuleAndDependenciesRecursively(moduleTypes, dependedModuleType);
		}

		public static List<Type> FindDependedModuleTypes(Type moduleType)
		{
			UgfExtensions.CheckUgfModuleType(moduleType);

			var depends =
				moduleType.GetCustomAttribute<DependsOnAttribute>() ?? new DependsOnAttribute();

			var dependencies = depends.DependedTypes.Distinct().ToList();

			return dependencies;
		}
	}
}