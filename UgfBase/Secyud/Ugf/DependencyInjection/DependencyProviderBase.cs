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

        public T TryGet<T>() where T : class
        {
            return TryGet(typeof(T)) as T;
        }

        public object TryGet(Type type)
        {
            InstanceDescriptor descriptor = GetInstanceDescriptor(type);
            return descriptor?.ObjectAccessor();
        }

        public virtual object Get(Type type)
        {
            InstanceDescriptor descriptor = GetInstanceDescriptor(type);

            if (descriptor is null)
                throw new UgfException($"Can't find dependency for exposed type {type}");

            return descriptor.ObjectAccessor();
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

                if (dd is null)
                    return null;

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