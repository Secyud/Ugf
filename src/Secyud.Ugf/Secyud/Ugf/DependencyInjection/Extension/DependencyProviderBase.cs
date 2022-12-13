using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Secyud.Ugf.DependencyInjection
{
    public abstract class DependencyProviderBase : IDependencyProvider
    {
        internal readonly IDependencyCollection DependencyCollection;
        protected readonly ConcurrentDictionary<Type, object> Instances;

        internal DependencyProviderBase(
            IDependencyCollection dependencyCollection,
            ConcurrentDictionary<Type, object> instances)
        {
            DependencyCollection = dependencyCollection;
            Instances = instances;
        }

        public virtual T GetDependency<T>() where T : class
        {
            return GetDependency(typeof(T)) as T;
        }

        public abstract object GetDependency(Type type);

        internal DependencyDescriptor GetDescriptor(Type type)
        {
            return DependencyCollection.ContainsKey(type) ? DependencyCollection[type] : null;
        }
        
        protected object CreateInstance(Type implementationType)
        {
            var constructor = implementationType.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0];

            var parameters = constructor
                .GetParameters()
                .Select(u => GetDependency(u.ParameterType)).ToArray();
            
            return constructor.Invoke(parameters);
        }
        protected object GetInstance(Type implementationType)
        {
            if (Instances.ContainsKey(implementationType))
                return Instances[implementationType];
            
            var instance = CreateInstance(implementationType);
            Instances[implementationType] = instance;
            return instance;
        }
    }
}