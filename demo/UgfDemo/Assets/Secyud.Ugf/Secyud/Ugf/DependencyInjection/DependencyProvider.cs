using System;
using System.Collections.Concurrent;

namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyProvider : DependencyProviderBase, IDependencyProvider, IDependencyScopeFactory
    {
        private readonly DependencyProviderBase _dependencyProvider;

        private readonly ConcurrentDictionary<Type, object> _instances;

        public DependencyProvider(DependencyProviderBase dependencyProvider)
        {
            _dependencyProvider = dependencyProvider;
            _instances = new ConcurrentDictionary<Type, object>();
        }

        public override object GetDependency(Type type)
        {
            var descriptor = _dependencyProvider.GetDescriptor(type);

            if (descriptor is null)
                throw new UgfException($"Could not find dependency: {type.FullName}!");

            switch (descriptor.DependencyLifeTime)
            {
                case DependencyLifeTime.Singleton:
                    return _dependencyProvider.GetDependency(type);
                case DependencyLifeTime.Scoped:
                    if (!_instances.ContainsKey(descriptor.ImplementationType))
                        _instances[descriptor.ImplementationType]
                            = descriptor.CreateInstance(this);
                    return _instances[descriptor.ImplementationType];
                case DependencyLifeTime.Transient:
                    return descriptor.CreateInstance(this);
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(DependencyLifeTime),
                        descriptor.DependencyLifeTime,
                        "DependencyLifeTime is invalid.");
            }
        }

        public override bool TryGetDependency(Type type, out object dependency)
        {
            var descriptor = _dependencyProvider.GetDescriptor(type);

            if (descriptor is null)
            {
                dependency = default;
                return false;
            }

            switch (descriptor.DependencyLifeTime)
            {
                case DependencyLifeTime.Singleton:
                    dependency = _dependencyProvider.GetDependency(type);
                    break;
                case DependencyLifeTime.Scoped:
                    if (!_instances.ContainsKey(descriptor.ImplementationType))
                        _instances[descriptor.ImplementationType]
                            = descriptor.CreateInstance(this);
                    dependency = _instances[descriptor.ImplementationType];
                    break;
                case DependencyLifeTime.Transient:
                    dependency = descriptor.CreateInstance(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(DependencyLifeTime),
                        descriptor.DependencyLifeTime,
                        "DependencyLifeTime is invalid.");
            }

            return true;
        }

        public IDependencyScope CreateScope()
        {
            return new DependencyScope(new DependencyProvider(this));
        }


        internal override DependencyDescriptor GetDescriptor(Type type)
        {
            return _dependencyProvider.GetDescriptor(type);
        }
    }
}