#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Ugf.Collections.Generic;

#endregion

namespace Secyud.Ugf.DependencyInjection
{
    [Registry(
        typeof(IDependencyProvider),
        typeof(IDependencyRegistrar)
    )]
    public class DependencyManager :
        DependencyProviderBase,
        IDependencyManager
    {
        private readonly IDependencyCollection _dependencyDescriptors;
        private readonly ConcurrentDictionary<Type, DependencyScopeProvider> _scopes = new();
        private readonly List<ITypeAnalyzer> _analyzers = new();

        internal DependencyManager(IDependencyCollection dependencyCollection = null)
        {
            _dependencyDescriptors = dependencyCollection ?? new DependencyCollection();
            AddType<DependencyManager>();
            RegisterInstance(this);
            _analyzers.Add(new DefaultTypeAnalyser());
        }

        public override DependencyDescriptor GetDependencyDescriptor(Type exposedType)
        {
            return _dependencyDescriptors[exposedType];
        }

        public void AddAssembly(Assembly assembly)
        {
            AddTypes(
                assembly.GetTypes().Where(type =>
                    type is
                    {
                        IsClass: true,
                        IsAbstract: false,
                        IsGenericType: false
                    }).ToArray());
        }

        public void AddTypes(params Type[] types)
        {
            foreach (Type type in types)
                AddType(type);
        }

        public void AddType<T>()
        {
            AddType(typeof(T));
        }

        public void AddType(Type type)
        {
            if (IsRegistryDisabled(type))
                return;

            RegistryAttribute registryAttr = type.GetCustomAttribute<RegistryAttribute>();

            foreach (ITypeAnalyzer analyzer in _analyzers)
                analyzer.AnalyzeType(type);

            if (registryAttr is null)
                CreateDependencyDescriptor(
                    type,
                    type,
                    RegistryAttribute.Transient
                );
            else
            {
                List<Type> exposedServiceTypes = ExposedServiceExplorer.GetExposedServices(type);

                foreach (Type exposedServiceType in exposedServiceTypes)
                    CreateDependencyDescriptor(
                        type,
                        exposedServiceType,
                        registryAttr
                    );
            }
        }

        private DependencyDescriptor CreateDependencyDescriptor(
            Type implementationType,
            Type exposedType,
            RegistryAttribute registryAttribute)
        {
            if (!_dependencyDescriptors.TryGetValue(implementationType,
                    out DependencyDescriptor descriptor))
            {
                if (exposedType == implementationType)
                {
                    descriptor = DependencyDescriptor.Describe(
                        implementationType, this,
                        new DependencyConstructor(implementationType), registryAttribute);
                    _dependencyDescriptors[exposedType] = descriptor;
                }
                else
                {
                    descriptor = CreateDependencyDescriptor(
                        implementationType, implementationType, registryAttribute);
                }
            }

            _dependencyDescriptors[exposedType] = descriptor;
            return descriptor;
        }

        protected override void HandleScope(DependencyDescriptor dd, InstanceDescriptor id)
        {
            id.ObjectAccessor = () => _scopes[dd.RegistryAttribute.DependScope].Get(dd.ImplementationType);
        }

        public void RegisterInstance<TExposed>(TExposed instance)
        {
            RegisterInstance(typeof(TExposed), instance);
        }

        public void RegisterInstance(Type type, object instance)
        {
            DependencyDescriptor descriptor = CreateDependencyDescriptor(
                instance.GetType(),
                type,
                RegistryAttribute.Default
            );
            descriptor.Instance = instance;
        }

        public void Register<T, TExposed>(DependencyLifeTime lifeTime = DependencyLifeTime.Singleton)
        {
            CreateDependencyDescriptor(
                typeof(T), typeof(TExposed),
                new RegistryAttribute { LifeTime = lifeTime });
        }

        public void RegisterCustom<T, TExposed>(IDependencyConstructor constructor,
            DependencyLifeTime lifeTime = DependencyLifeTime.Singleton)
        {
            _dependencyDescriptors[typeof(TExposed)] = DependencyDescriptor.Describe(
                typeof(T), this, constructor,
                new RegistryAttribute { LifeTime = lifeTime });
        }

        public IDependencyProvider CreateScopeProvider()
        {
            return new DependencyScopeProvider()
            {
                ParentProvider = this
            };
        }

        public void AddAnalyser(ITypeAnalyzer analyzer)
        {
            _analyzers.AddIfNotContains(analyzer);
        }

        private bool IsRegistryDisabled(Type type)
        {
            return type.IsDefined(typeof(RegistryDisabledAttribute));
        }

        public TScope CreateScope<TScope>() where TScope : DependencyScopeProvider
        {
            return CreateScope(typeof(TScope)) as TScope;
        }

        public DependencyScopeProvider CreateScope(Type scopeType)
        {
            if (!_scopes.TryGetValue(scopeType, out DependencyScopeProvider scope))
            {
                DependencyDescriptor dd = GetDependencyDescriptor(scopeType);
                DependencyScopeProvider pScope = null;
                Type dependScope = dd.RegistryAttribute.DependScope;
                if (dependScope is not null)
                    if (!_scopes.TryGetValue(dependScope, out pScope))
                        throw new UgfException("Depend scope is not available!");

                scope = Get(scopeType) as DependencyScopeProvider;

                _scopes[scopeType] = scope ?? throw new UgfException($"Cannot get scope: {scopeType}");

                if (pScope is null)
                    scope.ParentProvider = this;
                else
                {
                    scope.ParentProvider = pScope;
                    pScope.SubProviders.Add(scope);
                }
            }

            return scope;
        }

        public void DestroyScope<TScope>() where TScope : DependencyScopeProvider
        {
            if (!_scopes.TryGetValue(typeof(TScope), out DependencyScopeProvider scopeDescriptor))
                return;
            DestroyScope(scopeDescriptor);
        }

        private void DestroyScope(DependencyScopeProvider scopeDescriptor)
        {
            scopeDescriptor.Dispose();
            foreach (DependencyScopeProvider scope in scopeDescriptor.SubProviders)
                DestroyScope(scope);
        }

        public TScope GetScope<TScope>() where TScope : DependencyScopeProvider
        {
            if (!_scopes.TryGetValue(typeof(TScope), out DependencyScopeProvider scope))
                throw new UgfException($"Cannot get scope {typeof(TScope)} this time. Please ensure it is exist.");
            return scope as TScope;
        }
    }
}