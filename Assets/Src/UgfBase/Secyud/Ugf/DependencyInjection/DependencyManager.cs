#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        internal DependencyManager(IDependencyCollection dependencyCollection = null)
            : base(dependencyCollection ?? new DependencyCollection(),
                new())
        {
            Instances[GetType()] = this;
        }

        public override object Get(Type type)
        {
            DependencyDescriptor descriptor = GetDescriptor(type);

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
            foreach (Type type in types)
                AddType(type);
        }

        public void AddType(Type type)
        {
            if (IsConventionalRegistrationDisabled(type))
                return;

            DependencyLifeTime? lifeTime = type.GetLifeTimeOrNull();

            if (lifeTime == null)
                return;

            List<Type> exposedServiceTypes = ExposedServiceExplorer.GetExposedServices(type);

            foreach (Type exposedServiceType in exposedServiceTypes)
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