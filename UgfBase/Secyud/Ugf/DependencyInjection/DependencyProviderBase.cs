#region

using System;
using System.Collections.Concurrent;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    public abstract class DependencyProviderBase : IDependencyProvider,IRegistry
    {
        internal readonly ConcurrentDictionary<Type, InstanceDescriptor> InstanceDescriptor = new();

        internal DependencyProviderBase()
        {
            InstanceDescriptor[typeof(IDependencyProvider)] = new InstanceDescriptor(() => this);
        }

        public virtual T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        public virtual object Get(Type type)
        {
            InstanceDescriptor descriptor = GetInstanceDescriptor(type);

            if (descriptor is null)
            {
                U.LogWarning($"Can't find dependency for exposed type {type}");
            }

            return descriptor?.ObjectAccessor?.Invoke();
        }

        protected virtual void HandleScope(DependencyDescriptor dd, InstanceDescriptor id)
        {
            id.Instance = dd.Constructor.Construct(this);
            id.ObjectAccessor = () => id.Instance;
        }

        public abstract DependencyDescriptor GetDependencyDescriptor(Type exposedType);

        private InstanceDescriptor GetInstanceDescriptor(Type type)
        {
            if (!InstanceDescriptor.TryGetValue(type, out InstanceDescriptor id))
            {
                DependencyDescriptor dd = GetDependencyDescriptor(type);

                if (!InstanceDescriptor.TryGetValue(dd.ImplementationType, out id))
                {
                    id = new InstanceDescriptor();

                    switch (dd.RegistryAttribute.LifeTime)
                    {
                        case DependencyLifeTime.Singleton:
                            id.ObjectAccessor = () => dd.Instance;
                            break;
                        case DependencyLifeTime.Scoped:
                            HandleScope(dd, id);
                            break;
                        case DependencyLifeTime.Transient:
                            id.ObjectAccessor = () => dd.Constructor.Construct(this);
                            break;
                        default:
                            return null;
                    }

                    InstanceDescriptor[dd.ImplementationType] = id;
                }

                InstanceDescriptor[type] = id;
            }

            return id;
        }
    }
}