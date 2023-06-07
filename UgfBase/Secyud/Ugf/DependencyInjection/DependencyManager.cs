#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Secyud.Ugf.Resource;

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
        private readonly ClassManager _classManager = new();
        private readonly Dictionary<Type, ScopeDescriptor> _scopes = new();

        internal DependencyManager(IDependencyCollection dependencyCollection = null)
            : base(
                dependencyCollection ?? new DependencyCollection(),
                new Dictionary<Type, object>()
            )
        {
            Instances[GetType()] = this;
            AddSingleton(_classManager);
        }

        public override object Get(Type type)
        {
            DependencyDescriptor descriptor = GetDescriptor(type);
            return descriptor?.InstanceAccessor();
        }

        public void AddAssembly(Assembly assembly)
        {
            AddTypes(
                assembly.GetTypes()
                    .Where(
                        type =>
                            type is
                            {
                                IsClass: true,
                                IsAbstract: false,
                                IsGenericType: false
                            }
                    ).ToArray()
            );
        }

        public void AddTypes(params Type[] types)
        {
            foreach (Type type in types)
                AddType(type);
        }

        public void AddType(Type type)
        {
            _classManager.TryAddType(type);

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
                    lifeTime.Value
                );
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
                DependencyLifeTime.Singleton
            );
            Instances[instance.GetType()] = instance;
        }

        public void AddSingleton<T, TExposed>()
        {
            CreateDependencyDescriptor(
                typeof(T),
                typeof(TExposed),
                DependencyLifeTime.Singleton
            );
        }

        public void AddScoped<T, TExposed>()
        {
            CreateDependencyDescriptor(
                typeof(T),
                typeof(TExposed),
                DependencyLifeTime.Scoped
            );
        }

        public void AddTransient<T, TExposed>()
        {
            CreateDependencyDescriptor(
                typeof(T),
                typeof(TExposed),
                DependencyLifeTime.Transient
            );
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

        public TScope CreateScope<TScope>() where TScope :  DependencyScope
        {
            return CreateScope(typeof(TScope)) as TScope;
        }

        public DependencyScope CreateScope(Type scopeType)
        {
            if (!_scopes.TryGetValue(scopeType, out ScopeDescriptor scope))
            {
                DependScopeAttribute attr = scopeType.GetCustomAttribute<DependScopeAttribute>();
                ScopeDescriptor pScope = null;
                if (attr is not null)
                    if (!_scopes.TryGetValue(attr.DependScope, out pScope))
                        throw new UgfException("Depend scope is not available!");
                scope = new ScopeDescriptor(pScope);
                pScope?.SubScopes.Add(scope);
                _scopes[scopeType] = scope;
            }

            DependencyProviderBase provider = this;
            if (scope.ParentScope is not null)
            {
                if (scope.ParentScope.Scope is null)
                    throw new UgfException("Depend scope is not available this time!");
                provider = scope.ParentScope.Scope.Provider;
            }

            scope.Scope ??= provider.CreateInstance(scopeType) as DependencyScope;

            return scope.Scope;
        }

        public void DestroyScope<TScope>() where TScope : DependencyScope
        {
            if (!_scopes.TryGetValue(typeof(TScope), out ScopeDescriptor scopeDescriptor))
                return;
            DestroyScope(scopeDescriptor);
        }

        private void DestroyScope(ScopeDescriptor scopeDescriptor)
        {
            if (scopeDescriptor.Scope is null)
                return;
            scopeDescriptor.Scope?.Dispose();
            scopeDescriptor.Scope = null;
            foreach (ScopeDescriptor scope in scopeDescriptor.SubScopes)
                DestroyScope(scope);
        }

        public TScope GetScope<TScope>() where TScope : DependencyScope
        {
            if (!_scopes.TryGetValue(typeof(TScope), out ScopeDescriptor scope) ||
                scope.Scope is null)
                throw new UgfException($"Cannot get scope {typeof(TScope)} this time. Please ensure it is exist.");
            return scope.Scope as TScope;
        }
    }
}