using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyProvider : DependencyProviderBase, IDependencyProvider, IDependencyScopeFactory
    {
        private readonly DependencyProviderBase _dependencyProvider;

        public DependencyProvider(DependencyProviderBase dependencyProvider) 
            : base(new DependencyCollection(),
                new())
        {
            _dependencyProvider = dependencyProvider;
        }

        public override object GetDependency(Type type)
        {
            var descriptor = GetDescriptor(type);

            if (descriptor is null)
            {
                var originDescriptor = _dependencyProvider.GetDescriptor(type);
                
                descriptor = CreateDependencyDescriptor(
                    originDescriptor.ImplementationType,
                    type,
                    originDescriptor.DependencyLifeTime);
            }

            return descriptor.InstanceAccessor();
        }
        
        private DependencyDescriptor CreateDependencyDescriptor(
            Type implementationType,
            Type exposedType,
            DependencyLifeTime lifeTime)
        {
            var descriptor =
                lifeTime switch
                {
                    DependencyLifeTime.Singleton =>
                        DependencyDescriptor.Describe(
                            implementationType,
                            lifeTime,
                            () => _dependencyProvider.GetDependency(implementationType)
                        ),
                    DependencyLifeTime.Scoped =>
                        DependencyDescriptor.Describe(
                            implementationType,
                            lifeTime,
                            () => GetInstance(implementationType)
                        ),
                    DependencyLifeTime.Transient =>
                        DependencyDescriptor.Describe(
                            implementationType,
                            lifeTime,
                            () => CreateInstance(implementationType)
                        ),
                    _ => throw new ArgumentOutOfRangeException(nameof(lifeTime), lifeTime, null)
                };

            DependencyCollection[exposedType] = descriptor;
            return descriptor;
        }
        
        public IDependencyScope CreateScope()
        {
            return new DependencyScope(new DependencyProvider(this));
        }
    }
}