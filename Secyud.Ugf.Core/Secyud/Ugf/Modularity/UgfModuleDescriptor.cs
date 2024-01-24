using System;
using System.Collections.Generic;
using System.Reflection;

namespace Secyud.Ugf.Modularity
{
    public class UgfModuleDescriptor : IUgfModuleDescriptor
    {
        private readonly List<IUgfModuleDescriptor> _dependencies;

        public UgfModuleDescriptor(Type type, IUgfModule instance, bool isLoadedAsPlugIn)
        {
            Throw.IfNull(type);
            Throw.IfNull(instance);

            if (!type.GetTypeInfo().IsInstanceOfType(instance))
                throw new ArgumentException(
                    $"Given module instance ({instance.GetType().AssemblyQualifiedName}) is not an instance of given module type: {type.AssemblyQualifiedName}"
                );

            Type = type;
            Assembly = type.Assembly;
            Instance = instance;
            IsLoadedAsPlugIn = isLoadedAsPlugIn;

            _dependencies = new List<IUgfModuleDescriptor>();
        }

        public Type Type { get; }

        public Assembly Assembly { get; }

        public IUgfModule Instance { get; }

        public bool IsLoadedAsPlugIn { get; }


        public IReadOnlyList<IUgfModuleDescriptor> Dependencies => _dependencies;

        internal void AddDependency(IUgfModuleDescriptor descriptor)
        {
            if (!_dependencies.Contains(descriptor))
            {
                _dependencies.Add(descriptor);
            }
        }
    }
}