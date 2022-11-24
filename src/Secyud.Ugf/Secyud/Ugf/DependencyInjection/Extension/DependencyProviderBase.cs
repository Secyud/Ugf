using System;

namespace Secyud.Ugf.DependencyInjection
{
    public abstract class DependencyProviderBase : IDependencyProvider
    {
        public bool TryGetDependency<T>(out T dependency) where T : class
        {
            if (TryGetDependency(typeof(T), out var dependencyObj))
            {
                dependency = dependencyObj as T;
                return true;
            }

            dependency = default;
            return false;
        }

        public abstract bool TryGetDependency(Type type, out object dependency);

        public virtual T GetDependency<T>() where T : class
        {
            return GetDependency(typeof(T)) as T;
        }

        public abstract object GetDependency(Type type);
        internal abstract DependencyDescriptor GetDescriptor(Type type);
    }
}