#region

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using UnityEngine;

#endregion

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

        public virtual T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        public abstract object Get(Type type);

        internal DependencyDescriptor GetDescriptor(Type type)
        {
            _ = DependencyCollection.TryGetValue(type, out DependencyDescriptor provider);
            return provider;
        }

        protected object CreateInstance(Type implementationType)
        {
            ConstructorInfo constructor = implementationType.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();

            if (constructor is null)
                throw new UgfException($"Can not find constructor for type {implementationType}.");

            object[] parameters = constructor
                .GetParameters()
                .Select(u => Get(u.ParameterType)).ToArray();

#if DEBUG
            Debug.Log($"{implementationType} is constructing!");
#endif

            return constructor.Invoke(parameters);
        }

        protected object GetInstance(Type implementationType)
        {
            if (Instances.ContainsKey(implementationType))
                return Instances[implementationType];

            object instance = CreateInstance(implementationType);
            Instances[implementationType] = instance;
            return instance;
        }
    }
}