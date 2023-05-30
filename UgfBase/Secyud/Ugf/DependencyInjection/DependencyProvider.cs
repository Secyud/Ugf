#region

using System;
using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public class DependencyProvider : DependencyProviderBase, IDependencyProvider
    {
        private readonly IDependencyProviderFactory _dependencyProvider;

        private readonly Dictionary<Type, object> _instances;

        public DependencyProvider(IDependencyProviderFactory dependencyProvider)
            : base(new DependencyCollection(), new Dictionary<Type, object>())
        {
            _dependencyProvider = dependencyProvider;
        }
        public override object Get(Type type)
        {
            DependencyDescriptor descriptor = GetDescriptor(type);

            if (descriptor is null)
            {
                DependencyDescriptor originDescriptor = _dependencyProvider.GetDescriptor(type);

                descriptor = CreateDependencyDescriptor(
                    originDescriptor.ImplementationType,
                    type,
                    originDescriptor.DependencyLifeTime
                );
            }

            return descriptor.InstanceAccessor();
        }

        private DependencyDescriptor CreateDependencyDescriptor(
            Type implementationType,
            Type exposedType,
            DependencyLifeTime lifeTime)
        {
            DependencyDescriptor descriptor =
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