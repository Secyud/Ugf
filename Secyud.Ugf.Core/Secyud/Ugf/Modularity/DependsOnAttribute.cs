using System;
using System.Reflection;

namespace Secyud.Ugf.Modularity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependsOnAttribute : Attribute, IDependedTypesProvider
    {
        public DependsOnAttribute(params Type[] dependedModules)
        {
            DependedTypes = dependedModules ?? Type.EmptyTypes;
        }

        public Type[] DependedTypes { get; }

        public static Type[] GetDependModules(Type moduleType)
        {
            return moduleType.GetCustomAttribute<DependsOnAttribute>() 
                   ?.DependedTypes?? Type.EmptyTypes;
        }
    }
}