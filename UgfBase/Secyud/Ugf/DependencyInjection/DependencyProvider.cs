#region

using System;
using System.Collections.Concurrent;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyProvider : DependencyProviderBase, IDependencyProvider, IDependencyScopeFactory
    {
        private readonly DependencyProviderBase _dependencyProvider;

        private readonly ConcurrentDictionary<Type, object> _instances;

        public DependencyProvider(DependencyProviderBase dependencyProvider)
            : base(new DependencyCollection(),
                new ConcurrentDictionary<Type, object>())
        {
            _dependencyProvider = dependencyProvider;
        }

        public override object Get(Type type)
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

        public IDependencyScope CreateScope()
        {
            return new DependencyScope(new DependencyProvider(this));
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
                            () => _dependencyProvider.Get(implementationType)
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
    }
}