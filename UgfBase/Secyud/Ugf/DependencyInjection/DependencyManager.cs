#region

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.Archiving;
using UnityEngine;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    [ExposeType(
        typeof(IDependencyProvider),
        typeof(IDependencyRegistrar),
        typeof(IDependencyScopeFactory)
    )]
    public class DependencyManager :
        DependencyProviderBase,
        IDependencyManager,
        ISingleton
    {
        private readonly TypeManager _typeManager = new();

        internal DependencyManager(IDependencyCollection dependencyCollection = null)
            : base(dependencyCollection ?? new DependencyCollection(),
                new ConcurrentDictionary<Type, object>())
        {
            Instances[GetType()] = this;
            AddSingleton(_typeManager);
        }

        public override object Get(Type type)
        {
            var descriptor = GetDescriptor(type);

            if (descriptor is null)
                throw new UgfException($"Could not find dependency: {type.FullName}!");

            return descriptor.InstanceAccessor();
        }

        public IDependencyScope CreateScope()
        {
            return new DependencyScope(new DependencyProvider(this));
        }

        public void AddAssembly(Assembly assembly)
        {
            AddTypes(
                assembly.GetTypes()
                    .Where(type =>
                        type is
                        {
                            IsClass: true,
                            IsAbstract: false,
                            IsGenericType: false
                        }).ToArray()
            );
        }

        public void AddTypes(params Type[] types)
        {
            foreach (var type in types)
                AddType(type);
        }

        public void AddType(Type type)
        {

            Guid id = type.GetId();
            if (id != Guid.Empty)
            {
                if(_typeManager.TryGetValue(id, out var origin))
                    Debug.LogWarning($"Type manager: {type} replaced {origin.Type}");
                
                _typeManager[id] = new ConstructorContainer(type);
                return;
            }
            
            if (IsConventionalRegistrationDisabled(type))
                return;

            var lifeTime = type.GetLifeTimeOrNull();

            if (lifeTime == null)
                return;

            var exposedServiceTypes = ExposedServiceExplorer.GetExposedServices(type);

            foreach (var exposedServiceType in exposedServiceTypes)
                CreateDependencyDescriptor(
                    type,
                    exposedServiceType,
                    lifeTime.Value);
        }

        public void AddType<T>()
        {
            AddType(typeof(T));
        }

        public void AddSingleton<TExposed>(TExposed instance)
        {
            AddSingleton(typeof(TExposed), instance);
        }

        public void AddSingleton(Type type, object instance)
        {
            CreateDependencyDescriptor(
                instance.GetType(),
                type,
                DependencyLifeTime.Singleton);
            Instances[instance.GetType()] = instance;
        }

        public void AddSingleton<T, TExposed>()
        {
            CreateDependencyDescriptor(
                typeof(T),
                typeof(TExposed),
                DependencyLifeTime.Singleton);
        }

        public void AddScoped<T, TExposed>()
        {
            CreateDependencyDescriptor(
                typeof(T),
                typeof(TExposed),
                DependencyLifeTime.Scoped);
        }

        public void AddTransient<T, TExposed>()
        {
            CreateDependencyDescriptor(
                typeof(T),
                typeof(TExposed),
                DependencyLifeTime.Transient);
        }

        public void AddCustom<T, TExposed>(Func<object> instanceAccessor)
        {
            DependencyCollection[typeof(TExposed)]
                = DependencyDescriptor.Describe(
                    typeof(T),
                    DependencyLifeTime.Singleton,
                    instanceAccessor
                );
        }

        private bool IsConventionalRegistrationDisabled(Type type)
        {
            return type.IsDefined(typeof(DisableRegistrationAttribute), true);
        }

        private void CreateDependencyDescriptor(
            Type implementationType,
            Type exposedType,
            DependencyLifeTime lifeTime)
        {
            if (lifeTime == DependencyLifeTime.Transient)
                DependencyCollection[exposedType]
                    = DependencyDescriptor.Describe(
                        implementationType,
                        lifeTime,
                        () => CreateInstance(implementationType)
                    );
            else
                DependencyCollection[exposedType]
                    = DependencyDescriptor.Describe(
                        implementationType,
                        lifeTime,
                        () => GetInstance(implementationType)
                    );
        }
    }
}