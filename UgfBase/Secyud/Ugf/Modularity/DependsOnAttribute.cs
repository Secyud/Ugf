#region

using System;

#endregion

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
    }
}