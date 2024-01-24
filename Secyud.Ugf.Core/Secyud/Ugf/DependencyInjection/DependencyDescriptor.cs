using System;
using System.Diagnostics.CodeAnalysis;

namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyDescriptor
    {
        private readonly IDependencyManager _dependencyManager;

        private object _instance;

        public Type ImplementationType { get; private set; }

        public RegistryAttribute RegistryAttribute { get; private set; }

        public IDependencyConstructor Constructor { get; private set; }


        public object Instance
        {
            get { return _instance ??= Constructor.Construct(_dependencyManager); }
            set => _instance = value;
        }

        private DependencyDescriptor(IDependencyManager dependencyManager)
        {
            _dependencyManager = dependencyManager;
        }

        internal static DependencyDescriptor Describe(
            [NotNull] Type implementationType,
            [NotNull] IDependencyManager manager,
            [NotNull] IDependencyConstructor constructor,
            [NotNull] RegistryAttribute registryAttribute)
        {
            return new DependencyDescriptor(manager)
            {
                ImplementationType = implementationType,
                RegistryAttribute = registryAttribute,
                Constructor = constructor
            };
        }
    }
}